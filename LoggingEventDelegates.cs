using CMS.Core;
using CMS.EventLog;
using CMS.Membership;
using CMS.Websites;
using Microsoft.AspNetCore.Http;
using Mono.Cecil;
using System.ComponentModel;
using System.Diagnostics.Tracing;

namespace Customizations.Delegates
{
    public class LoggingEventDelegates
    {
        private static readonly IEventLogService _eventLogService = Service.Resolve<IEventLogService>();

        public static void WebPageEvents_Publish(object sender, PublishWebPageEventArgs e)
        {
            var data = new EventLogData(
                EventTypeEnum.Information,
                nameof(LoggingEventDelegates),
                "WebPagePublished");


            data.EventDescription = $"Web page '{e.Name}' with ID '{e.ID}' has been published. By user '{MembershipContext.AuthenticatedUser}'";
            data.EventTime = DateTime.Now;
            data.UserName = MembershipContext.AuthenticatedUser?.UserName ?? "Anonymous";

            _eventLogService.LogEvent(data);
        }

        public static void WebPageEvents_CreateLanguageVariant(object? sender, CreateWebPageLanguageVariantEventArgs e)
        {
            var data = new EventLogData(
                EventTypeEnum.Information,
                nameof(LoggingEventDelegates),
                "LanguageVariantCreated");


            data.EventDescription = $"Web page '{e.Name}' with ID '{e.ID}' was created. By user '{MembershipContext.AuthenticatedUser}'";
            data.EventTime = DateTime.Now;
            data.UserName = MembershipContext.AuthenticatedUser?.UserName ?? "Anonymous";

            _eventLogService.LogEvent(data);
        }

        public static void WebPageEvents_Delete(object? sender, DeleteWebPageEventArgs e)
        {
            var data = new EventLogData(
                EventTypeEnum.Information,
                nameof(LoggingEventDelegates),
                "WebPagePublished");


            data.EventDescription = $"Web page '{e.Name}' with ID '{e.ID}' has been published. By user '{MembershipContext.AuthenticatedUser}'";
            data.EventTime = DateTime.Now;
            data.UserName = MembershipContext.AuthenticatedUser?.UserName ?? "Anonymous";

            _eventLogService.LogEvent(data);
        }

        public static void WebPageEvents_Unpublish(object? sender, UnpublishWebPageEventArgs e)
        {
            var data = new EventLogData(
               EventTypeEnum.Information,
               nameof(LoggingEventDelegates),
               "WebPageUnpublished");


            data.EventDescription = $"Web page '{e.Name}' with ID '{e.ID}' has been unpublished. By user '{MembershipContext.AuthenticatedUser}'";
            data.EventTime = DateTime.Now;
            data.UserName = MembershipContext.AuthenticatedUser?.UserName ?? "Anonymous";

            _eventLogService.LogEvent(data);
        }

        public static void WebPageEvents_Create(object? sender, CreateWebPageEventArgs e)
        {
            var data = new EventLogData(
               EventTypeEnum.Information,
               nameof(LoggingEventDelegates),
               "WebPageCreate");


            data.EventDescription = $"Web page '{e.Name}' with ID '{e.ID}' has been created. By user '{MembershipContext.AuthenticatedUser}'";
            data.EventTime = DateTime.Now;
            data.UserName = MembershipContext.AuthenticatedUser?.UserName ?? "Anonymous";

            _eventLogService.LogEvent(data);
        }

        public static void WebPageEvents_UpdateDraft(object sender, UpdateWebPageDraftEventArgs e)
        {
            var data = new EventLogData(
               EventTypeEnum.Information,
               nameof(LoggingEventDelegates),
               "WebPageDraftUpdate");


            data.EventDescription = $"Web page draft '{e.Name}' with ID '{e.ID}' has been updated. By user '{MembershipContext.AuthenticatedUser}'";
            data.EventTime = DateTime.Now;
            data.UserName = MembershipContext.AuthenticatedUser?.UserName ?? "Anonymous";

            _eventLogService.LogEvent(data);
        }
    }
}
