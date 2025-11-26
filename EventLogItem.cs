using CMS.Core;
using CMS.EventLog;
using System;

namespace Customizations.Delegates
{
    /// <summary>
    /// Lightweight wrapper that holds both generic event-log fields and
    /// event-specific properties (page name / id). Can be converted to
    /// the CMS <see cref="EventLogData"/> when required by the background worker.
    /// </summary>
    public sealed class EventLogItem
    {
        public EventTypeEnum EventType { get; }
        public string EventSource { get; }
        public string EventCode { get; }
        public string MessageTemplate { get; }
        public string PageName { get; }
        public object PageId { get; }
        public string UserName { get; }
        public DateTime EventTime { get; }

        public EventLogItem(
            EventTypeEnum eventType,
            string eventSource,
            string eventCode,
            string messageTemplate,
            string pageName,
            object pageId,
            string userName,
            DateTime? eventTime = null)
        {
            EventType = eventType;
            EventSource = eventSource ?? throw new ArgumentNullException(nameof(eventSource));
            EventCode = eventCode ?? throw new ArgumentNullException(nameof(eventCode));
            MessageTemplate = messageTemplate ?? throw new ArgumentNullException(nameof(messageTemplate));
            PageName = pageName;
            PageId = pageId;
            UserName = userName ?? "Anonymous";
            EventTime = eventTime ?? DateTime.UtcNow;
        }

        /// <summary>
        /// Convert wrapper into CMS <see cref="EventLogData"/> instance.
        /// </summary>
        public EventLogData ToEventLogData()
        {
            var description = string.Format(MessageTemplate, PageName, PageId, UserName);

            return new EventLogData(EventType, EventSource, EventCode)
            {
                EventDescription = description,
                EventTime = EventTime,
                UserName = UserName
            };
        }

        /// <summary>
        /// Single-line textual representation suitable for file dump.
        /// </summary>
        public string ToLogLine()
        {
            // ISO 8601 for time, tab-separated fields, description last (may contain tabs/newlines).
            return $"{EventTime:O}\t{EventType}\t{EventSource}\t{EventCode}\t{UserName}\t{PageName}\t{PageId}\t{EscapeNewLines(EventLogDataSafeDescription())}";
        }

        private string EventLogDataSafeDescription()
        {
            try
            {
                return string.Format(MessageTemplate, PageName, PageId, UserName);
            }
            catch
            {
                return MessageTemplate;
            }
        }

        private static string EscapeNewLines(string input) =>
            input?.Replace("\r", "\\r").Replace("\n", "\\n") ?? string.Empty;
    }
}