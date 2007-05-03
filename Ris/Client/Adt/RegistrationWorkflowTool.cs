using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

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
                Execute(item, this.Context.DesktopWindow);
            }

            protected string OperationName
            {
                get { return _operationName; }
            }

            protected abstract bool Execute(RegistrationWorklistItem item, IDesktopWindow desktopWindow);

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
                return Execute(item, ctxt.DesktopWindow);
            }

            #endregion
        }

        [MenuAction("apply", "folderexplorer-items-contextmenu/Check-in")]
        [ClickHandler("apply", "Apply")]
        [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
        [ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
        [ExtensionOf(typeof(Folders.CheckedInFolder.DropHandlerExtensionPoint))]
        public class CheckInTool : WorkflowItemTool
        {
            public CheckInTool()
                : base("CheckInProcedure")
            {
            }

            protected override bool Execute(RegistrationWorklistItem item, IDesktopWindow desktopWindow)
            {
                try
                {
                    RequestedProcedureCheckInComponent checkInComponent = new RequestedProcedureCheckInComponent(item);
                    ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                        desktopWindow, checkInComponent, String.Format("Checking in {0}", Format.Custom(item.Name)));

                    if (exitCode == ApplicationComponentExitCode.Normal)
                    {
                        Platform.GetService<IRegistrationWorkflowService>(
                            delegate(IRegistrationWorkflowService service)
                            {
                                service.CheckInProcedure(new CheckInProcedureRequest(checkInComponent.SelectedRequestedProcedures));
                            });
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
        [ClickHandler("apply", "Apply")]
        [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
        [ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
        [ExtensionOf(typeof(Folders.CancelledFolder.DropHandlerExtensionPoint))]
        public class CancelTool : WorkflowItemTool, IDropHandler<RegistrationWorklistItem>
        {
            public CancelTool()
                : base("CancelOrder")
            {
            }

            protected override bool Execute(RegistrationWorklistItem item, IDesktopWindow desktopWindow)
            {
                try
                {
                    CancelOrderComponent cancelOrderComponent = new CancelOrderComponent(item.PatientProfileRef);
                    ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                        desktopWindow, cancelOrderComponent, String.Format("Cancel Order for {0}", Format.Custom(item.Name)));

                    if (exitCode == ApplicationComponentExitCode.Normal)
                    {
                        Platform.GetService<IRegistrationWorkflowService>(
                            delegate(IRegistrationWorkflowService service)
                            {
                                service.CancelOrder(new CancelOrderRequest(cancelOrderComponent.SelectedOrders, cancelOrderComponent.SelectedReason));
                            });
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
    
    }
}

