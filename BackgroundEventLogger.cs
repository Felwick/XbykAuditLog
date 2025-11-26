using CMS.Core;
using CMS.EventLog;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Customizations.Delegates
{
    internal sealed class BackgroundEventLogger : IDisposable
    {
        private static readonly Lazy<BackgroundEventLogger> _lazy = new(() => new BackgroundEventLogger());
        public static BackgroundEventLogger Instance => _lazy.Value;

        private readonly ConcurrentQueue<EventLogItem> _queue = new();
        private readonly ManualResetEventSlim _signal = new(false);
        private readonly CancellationTokenSource _cts = new();
        private readonly Task _worker;
        private readonly IEventLogService _eventLogService;
        private bool _disposed;

        private BackgroundEventLogger()
        {
            _eventLogService = Service.Resolve<IEventLogService>();
            _worker = Task.Factory.StartNew(ProcessQueue, TaskCreationOptions.LongRunning);
            AppDomain.CurrentDomain.ProcessExit += (_, _) => Dispose();
        }

        public void Enqueue(EventLogItem item)
        {
            if (_disposed || item is null) return;

            _queue.Enqueue(item);
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
                            var data = item.ToEventLogData();
                            _eventLogService.LogEvent(data);
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
                        var data = remaining.ToEventLogData();
                        _eventLogService.LogEvent(data);
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
                    // ignore worker wait failures
                }

                // Persist any remaining items to a file (best-effort).
                try
                {
                    if (!_queue.IsEmpty)
                    {
                        var logDir = Path.Combine(AppContext.BaseDirectory, "Logs");
                        Directory.CreateDirectory(logDir);

                        var fileName = $"BackgroundEventLoggerQueueDump_{DateTime.UtcNow:yyyyMMdd_HHmmss}.log";
                        var filePath = Path.Combine(logDir, fileName);

                        using var fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
                        using var writer = new StreamWriter(fs);

                        while (_queue.TryDequeue(out var item))
                        {
                            writer.WriteLine(item.ToLogLine());
                        }

                        writer.Flush();
                    }
                }
                catch
                {
                    // Swallow file I/O exceptions during shutdown to avoid throwing from Dispose.
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