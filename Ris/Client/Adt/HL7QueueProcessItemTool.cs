using System;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("process", "hl7Queue-contextmenu/Process Message")]
    [ButtonAction("process", "hl7Queue-toolbar/Process Message")]
    [ClickHandler("process", "Process")]
    [EnabledStateObserver("process", "Enabled", "EnabledChanged")]
    [Tooltip("process", "Process Message")]
    [IconSet("process", IconScheme.Colour, "HL7QueueProcessItemToolSmall.png", "HL7QueueProcessItemToolMedium.png", "HL7QueueProcessItemToolLarge.png")]

    [ExtensionOf(typeof(HL7QueueToolExtensionPoint))]
    public class HL7QueueProcessItemTool : Tool<IHL7QueueToolContext>
    {
        private bool _enabled;
        private event EventHandler _enabledChanged;

        public override void Initialize()
        {
            base.Initialize();
            this.Context.DefaultAction = Process;
            this.Context.SelectedHL7QueueItemChanged += delegate(object sender, EventArgs args)
            {
                this.Enabled = (this.Context.SelectedHL7QueueItem != null);
            };
        }

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler EnabledChanged
        {
            add { _enabledChanged += value; }
            remove { _enabledChanged -= value; }
        }

        public void Process()
        {
            ProcessQueueItem(this.Context.SelectedHL7QueueItem);
            this.Context.Refresh();
        }

        private void ProcessQueueItem(EntityRef<HL7QueueItem> queueItemRef)
        {
            try
            {
                IHL7QueueService service = ApplicationContext.GetService<IHL7QueueService>();
                HL7QueueItem queueItem = service.LoadHL7QueueItem(queueItemRef);

                try
                {
                    using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Update))
                    {
                        service.ProcessHL7QueueItem(queueItem);
                        service.SetHL7QueueItemComplete(queueItem);
                        scope.Complete();
                    }
                }
                catch (Exception e)
                {
                    Platform.Log("Unable to process HL7 queue item: " + queueItem.ToString());
                    Platform.Log("Exception thrown: " + e.Message);
                    service.SetHL7QueueItemError(queueItem, e.Message);
                }          
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to process queue item:  " + e.Message);
            }
        }
    }
}
