using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common.ProtocollingWorkflow;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("apply", "folderexplorer-items-contextmenu/Add Protocol (Testing only)", "AddProtocol")]
    [IconSet("apply", IconScheme.Colour, "AddToolSmall.png", "AddToolMedium.png", "AddToolLarge.png")]
    //[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
    [ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
    public class ProtocolTestTool : Tool<IRegistrationWorkflowItemToolContext>
    {
        //private bool _enabled;
        //private event EventHandler _enabledChanged;


        //public ProtocolTestTool()
        //{
        //    _enabled = true;
        //}

        //public bool Enabled
        //{
        //    get { return _enabled; }
        //    protected set
        //    {
        //        if (_enabled != value)
        //        {
        //            _enabled = value;
        //            EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
        //        }
        //    }
        //}

        //public event EventHandler EnabledChanged
        //{
        //    add { _enabledChanged += value; }
        //    remove { _enabledChanged -= value; }
        //}

        public void AddProtocol()
        {
            RegistrationWorklistItem item = CollectionUtils.FirstElement<RegistrationWorklistItem>(this.Context.SelectedItems);
            if(item != null)
            {
                try
                {
                    Platform.GetService<IProtocollingWorkflowService>(
                        delegate(IProtocollingWorkflowService service)
                            {
                                service.AddProtocol(new AddProtocolRequest(item.OrderRef));
                            });
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, this.Context.DesktopWindow);
                }
            }

        }

    }
}
