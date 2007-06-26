using System;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.HL7Admin;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("process", "hl7Queue-contextmenu/Process Message")]
    [ButtonAction("process", "hl7Queue-toolbar/Process Message")]
    [ClickHandler("process", "Process")]
    [EnabledStateObserver("process", "Enabled", "EnabledChanged")]
    [Tooltip("process", "Process Message")]
	[IconSet("process", IconScheme.Colour, "ProcessMessageToolSmall.png", "ProcessMessageToolMedium.png", "ProcessMessageToolLarge.png")]

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
            //TODO:  Wait cursor
            ProcessQueueItem(this.Context.SelectedHL7QueueItem);
            this.Context.Refresh();
        }

        private void ProcessQueueItem(HL7QueueItemDetail selectedQueueItem)
        {
            try
            {
                Platform.GetService<IHL7QueueService>(
                    delegate(IHL7QueueService service)
                    {
                        ProcessHL7QueueItemRequest processRequest = new ProcessHL7QueueItemRequest(selectedQueueItem.QueueItemRef);
                        service.ProcessHL7QueueItem(processRequest);
                    });
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, Context.DesktopWindow);
            }
        }
    }
}
