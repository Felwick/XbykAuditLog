using CMS.Core;
using CMS.EventLog;
using CMS.Membership;
using CMS.Websites;
using System;

namespace Customizations.Delegates
{
    public class LoggingEventDelegates
    {
        private static void EnqueueWebPageEvent(EventTypeEnum eventType, string eventCode, string messageTemplate, string name, object id)
        {
            var userName = MembershipContext.AuthenticatedUser?.UserName ?? "Anonymous";

            var item = new EventLogItem(
                eventType: eventType,
                eventSource: nameof(LoggingEventDelegates),
                eventCode: eventCode,
                messageTemplate: messageTemplate,
                pageName: name,
                pageId: id,
                userName: userName,
                eventTime: DateTime.UtcNow);

            BackgroundEventLogger.Instance.Enqueue(item);
        }

        public static void WebPageEvents_Publish(object sender, PublishWebPageEventArgs e)
        {
            EnqueueWebPageEvent(EventTypeEnum.Information, "WebPagePublished",
                "Web page '{0}' with ID '{1}' has been published. By user '{2}'",
                e.Name, e.ID);
        }

        public static void WebPageEvents_CreateLanguageVariant(object? sender, CreateWebPageLanguageVariantEventArgs e)
        {
            EnqueueWebPageEvent(EventTypeEnum.Information, "LanguageVariantCreated",
                "Web page '{0}' with ID '{1}' was created. By user '{2}'",
                e.Name, e.ID);
        }

        public static void WebPageEvents_Delete(object? sender, DeleteWebPageEventArgs e)
        {
            EnqueueWebPageEvent(EventTypeEnum.Information, "WebPageDeleted",
                "Web page '{0}' with ID '{1}' has been deleted. By user '{2}'",
                e.Name, e.ID);
        }

        public static void WebPageEvents_Unpublish(object? sender, UnpublishWebPageEventArgs e)
        {
            EnqueueWebPageEvent(EventTypeEnum.Information, "WebPageUnpublished",
                "Web page '{0}' with ID '{1}' has been unpublished. By user '{2}'",
                e.Name, e.ID);
        }

        public static void WebPageEvents_Create(object? sender, CreateWebPageEventArgs e)
        {
            EnqueueWebPageEvent(EventTypeEnum.Information, "WebPageCreated",
                "Web page '{0}' with ID '{1}' has been created. By user '{2}'",
                e.Name, e.ID);
        }

        public static void WebPageEvents_UpdateDraft(object sender, UpdateWebPageDraftEventArgs e)
        {
            EnqueueWebPageEvent(EventTypeEnum.Information, "WebPageDraftUpdate",
                "Web page draft '{0}' with ID '{1}' has been updated. By user '{2}'",
                e.Name, e.ID);
        }
    }
}
