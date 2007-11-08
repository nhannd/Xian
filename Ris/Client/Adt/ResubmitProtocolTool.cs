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
    [MenuAction("apply", "folderexplorer-items-contextmenu/Resolve Protocol - Resubmit", "ResolveProtocol")]
    [IconSet("apply", IconScheme.Colour, "AddToolSmall.png", "AddToolMedium.png", "AddToolLarge.png")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
    [ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
    public class ResubmitProtocolTool : Tool<IRegistrationWorkflowItemToolContext>
    {
        public void ResolveProtocol()
        {
            RegistrationWorklistItem item = CollectionUtils.FirstElement<RegistrationWorklistItem>(this.Context.SelectedItems);
            if (item != null)
            {
                try
                {
                    Platform.GetService<IProtocollingWorkflowService>(
                        delegate(IProtocollingWorkflowService service)
                        {
                            service.ResubmitProtocol(new ResubmitProtocolRequest(item.OrderRef));
                        });
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, this.Context.DesktopWindow);
                }
            }
        }

        public virtual bool Enabled
        {
            get
            {
                bool enabled = false;

                RegistrationWorklistItem item = CollectionUtils.FirstElement<RegistrationWorklistItem>(this.Context.SelectedItems);
                if (item != null)
                {
                    try
                    {
                        Platform.GetService<IProtocollingWorkflowService>(
                            delegate(IProtocollingWorkflowService service)
                            {
                                GetClericalProtocolOperationEnablementResponse response =
                                service.GetClericalProtocolOperationEnablement(new GetClericalProtocolOperationEnablementRequest(item.OrderRef));

                                enabled = response.CanResolveByResubmit;
                            });
                    }
                    catch (Exception e)
                    {
                        ExceptionHandler.Report(e, this.Context.DesktopWindow);
                    }
                }

                return enabled;
            }
        }

        public virtual event EventHandler EnabledChanged
        {
            add { this.Context.SelectedItemsChanged += value; }
            remove { this.Context.SelectedItemsChanged -= value; }
        }
    }

    [MenuAction("apply", "folderexplorer-items-contextmenu/Resolve Protocol - Cancel Order", "ResolveProtocol")]
    [IconSet("apply", IconScheme.Colour, "AddToolSmall.png", "AddToolMedium.png", "AddToolLarge.png")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
    [ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
    public class CancelProtcolTool : Tool<IRegistrationWorkflowItemToolContext>
    {
        public void ResolveProtocol()
        {
            RegistrationWorklistItem item = CollectionUtils.FirstElement<RegistrationWorklistItem>(this.Context.SelectedItems);
            if (item != null)
            {
                try
                {
                    Platform.GetService<IProtocollingWorkflowService>(
                        delegate(IProtocollingWorkflowService service)
                        {
                            service.CancelProtocolAndOrder(new CancelProtocolAndOrderRequest(item.OrderRef));
                        });
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, this.Context.DesktopWindow);
                }
            }
        }

        public virtual bool Enabled
        {
            get
            {
                bool enabled = false;

                RegistrationWorklistItem item = CollectionUtils.FirstElement<RegistrationWorklistItem>(this.Context.SelectedItems);
                if (item != null)
                {
                    try
                    {
                        Platform.GetService<IProtocollingWorkflowService>(
                            delegate(IProtocollingWorkflowService service)
                            {
                                GetClericalProtocolOperationEnablementResponse response =
                                service.GetClericalProtocolOperationEnablement(new GetClericalProtocolOperationEnablementRequest(item.OrderRef));

                                enabled = response.CanResolveByCancel;
                            });
                    }
                    catch (Exception e)
                    {
                        ExceptionHandler.Report(e, this.Context.DesktopWindow);
                    }
                }

                return enabled;
            }
        }

        public virtual event EventHandler EnabledChanged
        {
            add { this.Context.SelectedItemsChanged += value; }
            remove { this.Context.SelectedItemsChanged -= value; }
        }
    }
}
