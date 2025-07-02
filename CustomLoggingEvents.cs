using CMS;
using CMS.DataEngine;
using CMS.DataEngine.Query;
using CMS.EventLog;
using CMS.Websites;
using Customizations;
using Customizations.Delegates;

[assembly: RegisterModule(typeof(CustomLoggingEvents))]
namespace Customizations
{

    public class CustomLoggingEvents : Module
    {
        public CustomLoggingEvents() : base(nameof(CustomLoggingEvents))
        {
        }

        protected override void OnInit()
        {
            base.OnInit();
            
            WebPageEvents.UpdateDraft.After += LoggingEventDelegates.WebPageEvents_UpdateDraft;
            WebPageEvents.Create.After += LoggingEventDelegates.WebPageEvents_Create;
            WebPageEvents.Unpublish.Execute += LoggingEventDelegates.WebPageEvents_Unpublish;
            WebPageEvents.Delete.Execute += LoggingEventDelegates.WebPageEvents_Delete;
            WebPageEvents.CreateLanguageVariant.After += LoggingEventDelegates.WebPageEvents_CreateLanguageVariant;
            WebPageEvents.Publish.Execute += LoggingEventDelegates.WebPageEvents_Publish;

        }
        
    }
}
