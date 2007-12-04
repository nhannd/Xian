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
    [ExtensionOf(typeof(RegistrationMainWorkflowItemToolExtensionPoint))]
    public class ProtocolTestTool : Tool<IRegistrationWorkflowItemToolContext>
    {
        public void AddProtocol()
        {
            RegistrationWorklistItem item = CollectionUtils.FirstElement(this.Context.SelectedItems);
            if(item != null)
            {
                try
                {
                    Platform.GetService<IProtocollingWorkflowService>(
                        delegate(IProtocollingWorkflowService service)
                            {
                                service.AddOrderProtocolSteps(new AddOrderProtocolStepsRequest(item.OrderRef));
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
