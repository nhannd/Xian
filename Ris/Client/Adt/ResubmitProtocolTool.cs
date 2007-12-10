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
    [ExtensionOf(typeof(RegistrationBookingWorkflowItemToolExtensionPoint))]
    public class ResubmitProtocolTool : Tool<IRegistrationWorkflowItemToolContext>
    {
        public void ResolveProtocol()
        {
            RegistrationWorklistItem item = CollectionUtils.FirstElement(this.Context.SelectedItems);
            if (item != null)
            {
                try
                {
                    Platform.GetService<IProtocollingWorkflowService>(
                        delegate(IProtocollingWorkflowService service)
                        {
                            service.ResubmitProtocol(new ResubmitProtocolRequest(item.OrderRef));

                            IRegistrationWorkflowItemToolContext context = (IRegistrationWorkflowItemToolContext)this.ContextBase;

                            IFolder suspendedProtocolFolder = CollectionUtils.SelectFirst<IFolder>(context.Folders,
                                delegate(IFolder f) { return f is Folders.SuspendedProtocolFolder; });

                            if (suspendedProtocolFolder != null)
                            {
                                if (suspendedProtocolFolder.IsOpen)
                                    suspendedProtocolFolder.Refresh();
                                else
                                    suspendedProtocolFolder.RefreshCount();
                            }
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

                RegistrationWorklistItem item = CollectionUtils.FirstElement(this.Context.SelectedItems);
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
            add { this.Context.SelectionChanged += value; }
            remove { this.Context.SelectionChanged -= value; }
        }
    }

    [MenuAction("apply", "folderexplorer-items-contextmenu/Resolve Protocol - Cancel Order", "ResolveProtocol")]
    [IconSet("apply", IconScheme.Colour, "AddToolSmall.png", "AddToolMedium.png", "AddToolLarge.png")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
    [ExtensionOf(typeof(RegistrationBookingWorkflowItemToolExtensionPoint))]
    public class CancelProtocolTool : Tool<IRegistrationWorkflowItemToolContext>
    {
        public void ResolveProtocol()
        {
            RegistrationWorklistItem item = CollectionUtils.FirstElement(this.Context.SelectedItems);
            if (item != null)
            {
                try
                {
                    Platform.GetService<IProtocollingWorkflowService>(
                        delegate(IProtocollingWorkflowService service)
                        {
                            service.CancelProtocolAndOrder(new CancelProtocolAndOrderRequest(item.OrderRef));

                            IRegistrationWorkflowItemToolContext context = (IRegistrationWorkflowItemToolContext)this.ContextBase;

                            IFolder rejectedProtocolFolder = CollectionUtils.SelectFirst<IFolder>(context.Folders,
                                delegate(IFolder f) { return f is Folders.RejectedProtocolFolder; });

                            if (rejectedProtocolFolder != null)
                            {
                                if (rejectedProtocolFolder.IsOpen)
                                    rejectedProtocolFolder.Refresh();
                                else
                                    rejectedProtocolFolder.RefreshCount();
                            }
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

                RegistrationWorklistItem item = CollectionUtils.FirstElement(this.Context.SelectedItems);
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
            add { this.Context.SelectionChanged += value; }
            remove { this.Context.SelectionChanged -= value; }
        }
    }
}
