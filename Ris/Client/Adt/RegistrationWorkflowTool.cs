using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Adt
{
    public class RegistrationWorkflowTool
    {
        public abstract class WorkflowItemTool : Tool<IRegistrationWorkflowItemToolContext>, IDropHandler<RegistrationWorklistItem>
        {
            protected string _operationName;

            public WorkflowItemTool(string operationName)
            {
                _operationName = operationName;
            }

            public virtual bool Enabled
            {
                get
                {
                    return this.Context.GetWorkflowOperationEnablement(_operationName);
                }
            }

            public virtual event EventHandler EnabledChanged
            {
                add { this.Context.SelectedItemsChanged += value; }
                remove { this.Context.SelectedItemsChanged -= value; }
            }

            public virtual void Apply()
            {
                RegistrationWorklistItem item = CollectionUtils.FirstElement<RegistrationWorklistItem>(this.Context.SelectedItems);
                bool success = Execute(item, this.Context.DesktopWindow, this.Context.Folders);
                if (success)
                {
                    this.Context.SelectedFolder.Refresh();
                }
            }

            protected string OperationName
            {
                get { return _operationName; }
            }

            protected abstract bool Execute(RegistrationWorklistItem item, IDesktopWindow desktopWindow, IEnumerable folders);

            #region IDropHandler<RegistrationWorklistItem> Members

            public virtual bool CanAcceptDrop(IDropContext dropContext, ICollection<RegistrationWorklistItem> items)
            {
                IRegistrationWorkflowFolderDropContext ctxt = (IRegistrationWorkflowFolderDropContext)dropContext;
                return ctxt.GetOperationEnablement(this.OperationName);
            }

            public virtual bool ProcessDrop(IDropContext dropContext, ICollection<RegistrationWorklistItem> items)
            {
                IRegistrationWorkflowFolderDropContext ctxt = (IRegistrationWorkflowFolderDropContext)dropContext;
                RegistrationWorklistItem item = CollectionUtils.FirstElement<RegistrationWorklistItem>(items);
                bool success = Execute(item, ctxt.DesktopWindow, ctxt.FolderSystem.Folders);
                if (success)
                {
                    ctxt.FolderSystem.SelectedFolder.Refresh();
                    return true;
                }
                return false;
            }

            #endregion
        }

        [MenuAction("apply", "folderexplorer-items-contextmenu/Check-in")]
        [ButtonAction("apply", "folderexplorer-items-toolbar/Check-in")]
        [ClickHandler("apply", "Apply")]
		[IconSet("apply", IconScheme.Colour, "Icons.CheckInToolSmall.png", "Icons.CheckInToolMedium.png", "Icons.CheckInToolLarge.png")]
		[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
        [ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
        [ExtensionOf(typeof(Folders.CheckedInFolder.DropHandlerExtensionPoint))]
        public class CheckInTool : WorkflowItemTool
        {
            public CheckInTool()
                : base("CheckInProcedure")
            {
            }

            protected override bool Execute(RegistrationWorklistItem item, IDesktopWindow desktopWindow, IEnumerable folders)
            {
                try
                {
                    CheckInOrderComponent checkInComponent = new CheckInOrderComponent(item); 
                    ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                        desktopWindow,
                        checkInComponent, 
                        String.Format("Checking in {0}", PersonNameFormat.Format(item.Name)));

                    if (exitCode == ApplicationComponentExitCode.Normal)
                    {
                        Platform.GetService<IRegistrationWorkflowService>(
                            delegate(IRegistrationWorkflowService service)
                            {
                                service.CheckInProcedure(new CheckInProcedureRequest(checkInComponent.SelectedOrders));
                            });

                        IFolder checkInFolder = CollectionUtils.SelectFirst<IFolder>(folders,
                            delegate(IFolder f) { return f is Folders.CheckedInFolder; });
                        checkInFolder.RefreshCount();

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, desktopWindow);
                    return false;
                }
            }
        }

        [MenuAction("apply", "folderexplorer-items-contextmenu/Cancel")]
        [ButtonAction("apply", "folderexplorer-items-toolbar/Cancel")]
        [ClickHandler("apply", "Apply")]
		[IconSet("apply", IconScheme.Colour, "Icons.DeleteToolSmall.png", "Icons.DeleteToolMedium.png", "Icons.DeleteToolLarge.png")]
		[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
        [ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
        [ExtensionOf(typeof(Folders.CancelledFolder.DropHandlerExtensionPoint))]
        public class CancelTool : WorkflowItemTool, IDropHandler<RegistrationWorklistItem>
        {
            public CancelTool()
                : base("CancelOrder")
            {
            }

            protected override bool Execute(RegistrationWorklistItem item, IDesktopWindow desktopWindow, IEnumerable folders)
            {
                try
                {
                    CancelOrderComponent cancelOrderComponent = new CancelOrderComponent(item);
                    ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                        desktopWindow,
                        cancelOrderComponent, 
                        String.Format(SR.TitleCancelOrder, PersonNameFormat.Format(item.Name)));

                    if (exitCode == ApplicationComponentExitCode.Normal)
                    {
                        Platform.GetService<IRegistrationWorkflowService>(
                            delegate(IRegistrationWorkflowService service)
                            {
                                service.CancelOrder(new CancelOrderRequest(cancelOrderComponent.SelectedOrders, cancelOrderComponent.SelectedReason));
                            });

                        IFolder cancelledFolder = CollectionUtils.SelectFirst<IFolder>(folders,
                           delegate(IFolder f) { return f is Folders.CancelledFolder; });
                        cancelledFolder.RefreshCount();

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, desktopWindow);
                    return false;
                }
            }
        }

        [MenuAction("apply", "folderexplorer-items-contextmenu/Replace Order")]
        [ButtonAction("apply", "folderexplorer-items-toolbar/Replace Order")]
        [ClickHandler("apply", "Apply")]
        [IconSet("apply", IconScheme.Colour, "AddToolSmall.png", "AddToolMedium.png", "AddToolLarge.png")]
        [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
        [ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
        public class RegistrationReplaceOrderTool : WorkflowItemTool
        {
            public RegistrationReplaceOrderTool()
                : base("ReplaceOrder")
            {
            }

            protected override bool Execute(RegistrationWorklistItem item, IDesktopWindow desktopWindow, IEnumerable folders)
            {
                OrderDetail existingOrder = null;

                try
                {
                    Platform.GetService<IOrderEntryService>(
                        delegate(IOrderEntryService service)
                            {
                                LoadOrderDetailResponse response = service.LoadOrderDetail(new LoadOrderDetailRequest(item.AccessionNumber));
                                existingOrder = response.OrderDetail;
                            });
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, desktopWindow);
                    return false;
                }

                if (desktopWindow.ShowMessageBox(SR.MessageReplaceOrder, MessageBoxActions.OkCancel) == DialogBoxAction.Ok)
                {
                    ApplicationComponent.LaunchAsWorkspace(
                        desktopWindow,
                        new OrderEntryComponent(existingOrder),
                        string.Format(SR.TitleNewOrder, PersonNameFormat.Format(item.Name), MrnFormat.Format(item.Mrn)),
                        delegate(IApplicationComponent c)
                        {
                            if (c.ExitCode == ApplicationComponentExitCode.Normal)
                            {
                                try
                                {
                                    OrderEntryComponent component = (OrderEntryComponent)c;

                                    Platform.GetService<IRegistrationWorkflowService>(
                                        delegate(IRegistrationWorkflowService service)
                                        {
                                            service.ReplaceOrder(new ReplaceOrderRequest(component.PlaceOrderRequest, component.CancelOrderRequest));
                                        });

                                    IFolder cancelledFolder = CollectionUtils.SelectFirst<IFolder>(folders,
                                       delegate(IFolder f) { return f is Folders.CancelledFolder; });

                                    cancelledFolder.RefreshCount();
                                }
                                catch (Exception e)
                                {
                                    ExceptionHandler.Report(e, SR.ExceptionCannotReplaceOrder, this.Context.DesktopWindow);
                                }
                            }
                        });
                }

                return true;
            }
        }
    }
}

