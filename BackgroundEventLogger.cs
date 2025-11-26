using CMS.Core;
using CMS.EventLog;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Customizations.Delegates
{
    internal sealed class BackgroundEventLogger : IDisposable
    {
        private static readonly Lazy<BackgroundEventLogger> _lazy = new(() => new BackgroundEventLogger());
        public static BackgroundEventLogger Instance => _lazy.Value;

        private readonly ConcurrentQueue<EventLogData> _queue = new();
        private readonly ManualResetEventSlim _signal = new(false);
        private readonly CancellationTokenSource _cts = new();
        private readonly Task _worker;
        private readonly IEventLogService _eventLogService;
        private bool _disposed;

        private BackgroundEventLogger()
        {
            _eventLogService = Service.Resolve<IEventLogService>();
            _worker = Task.Factory.StartNew(ProcessQueue, TaskCreationOptions.LongRunning);
            AppDomain.CurrentDomain.ProcessExit += (_,_) => Dispose();
        }

        public void Enqueue(EventLogData data)
        {
            if (_disposed) return;

            _queue.Enqueue(data);
            _signal.Set();
        }

        private void ProcessQueue()
        {
            var token = _cts.Token;

            try
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        _signal.Wait(token);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }

                    while (_queue.TryDequeue(out var item))
                    {
                        try
                        {
                            _eventLogService.LogEvent(item);
                        }
                        catch
                        {
                            // Swallow logging exceptions to avoid crashing the background worker.
                            // Consider recording failures to a fallback store if needed.
                        }

                        if (token.IsCancellationRequested) break;
                    }

                    _signal.Reset();
                }

                // Final best-effort drain
                while (_queue.TryDequeue(out var remaining))
                {
                    try
                    {
                        _eventLogService.LogEvent(remaining);
                    }
                    catch
                    {
                        // ignore
                    }
                }
            }
            catch
            {
                // Defensive: ensure background thread never throws unhandled exceptions.
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            try
            {
                _cts.Cancel();
                _signal.Set();

                try
                {
                    _worker.Wait(TimeSpan.FromSeconds(5));
                }
                catch
                {
                    // ignore
                }
            }
            finally
            {
                _cts.Dispose();
                _signal.Dispose();
            }
        }
    }
}