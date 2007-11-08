using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common.Admin.ProtocolAdmin;
using ClearCanvas.Ris.Application.Common.ProtocollingWorkflow;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("apply", "folderexplorer-items-contextmenu/Add Protocol (Testing only)", "AddProtocol")]
    [IconSet("apply", IconScheme.Colour, "AddToolSmall.png", "AddToolMedium.png", "AddToolLarge.png")]
    [ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
    public class ProtocolTestTool : Tool<IRegistrationWorkflowItemToolContext>
    {
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

    [MenuAction("apply", "folderexplorer-items-contextmenu/Add Protocol Code(Testing only)", "AddProtocolCode")]
    [IconSet("apply", IconScheme.Colour, "AddToolSmall.png", "AddToolMedium.png", "AddToolLarge.png")]
    [ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
    public class ProtocolCodeTestTool : Tool<IRegistrationWorkflowItemToolContext>
    {
        public void AddProtocolCode()
        {
            try
            {
                Platform.GetService<IProtocolAdminService>(
                    delegate(IProtocolAdminService service)
                    {
                        service.AddProtocolCode(new AddProtocolCodeRequest("TestCode" + DateTime.Now.ToShortTimeString(), "Description"));
                    });
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Context.DesktopWindow);
            }
        }

    }
}
