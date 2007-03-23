using System;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.HL7Admin;

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

        private void ProcessQueueItem(HL7QueueItemDetail selectedQueueItem)
        {
            try
            {
                try
                {
                    //TODO:  combine Process..(), Set...Complete(), and Set...Error()

                    //using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Update))
                    //{
                    //    //service.ProcessHL7QueueItem(queueItem);
                    //    ProcessHL7QueueItemRequest processRequest = new ProcessHL7QueueItemRequest(selectedQueueItem.QueueItemRef);
                    //    ProcessHL7QueueItemResponse processResponse;
                    //    Platform.GetService<IHL7QueueService>(
                    //        delegate(IHL7QueueService service)
                    //        {
                    //            try
                    //            {
                    //                processResponse = service.ProcessHL7QueueItem(processRequest);
                    //            }
                    //            catch (Exception e)
                    //            {
                    //                ExceptionHandler.Report(e, desktopwindow);
                    //            }
                    //        });

                    //    //service.SetHL7QueueItemComplete(queueItem);
                    //    SetHL7QueueItemCompleteRequest completeRequest = new SetHL7QueueItemCompleteRequest(selectedQueueItem.QueueItemRef);
                    //    SetHL7QueueItemCompleteResponse completeResponse;
                    //    Platform.GetService<IHL7QueueService>(
                    //        delegate(IHL7QueueService service)
                    //        {
                    //            try
                    //            {
                    //                completeResponse = service.SetHL7QueueItemComplete(completeRequest);
                    //            }
                    //            catch (Exception e)
                    //            {
                    //                ExceptionHandler.Report(e, desktopwindow);
                    //            }
                    //        });

                    //    scope.Complete();
                    //}
                }
                catch (Exception e)
                {
                    //Platform.Log("Unable to process HL7 queue item: " + queueItem.ToString());
                    //Platform.Log("Exception thrown: " + e.Message);                    
                    
                    ////service.SetHL7QueueItemError(queueItem, e.Message);
                    //SetHL7QueueItemErrorRequest errorRequest = new SetHL7QueueItemErrorRequest(selectedQueueItem.QueueItemRef, e.Message);
                    //SetHL7QueueItemErrorResponse errorResponse;
                    //Platform.GetService<IHL7QueueService>(
                    //    delegate(IHL7QueueService service)
                    //    {
                    //        try
                    //        {
                    //            errorResponse = service.SetHL7QueueItemError(errorRequest);
                    //        }
                    //        catch (Exception e)
                    //        {
                    //            ExceptionHandler.Report(e, desktopwindow);
                    //        }
                    //    });
                }          
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to process queue item:  " + e.Message);
            }
        }
    }
}
