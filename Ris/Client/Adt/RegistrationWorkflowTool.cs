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
using System.Collections;
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
                        desktopWindow, checkInComponent, String.Format("Checking in {0}", PersonNameFormat.Format(item.Name)));

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
                    CancelOrderComponent cancelOrderComponent = new CancelOrderComponent(item.PatientProfileRef);
                    ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                        desktopWindow, cancelOrderComponent, String.Format("Cancel Order for {0}", PersonNameFormat.Format(item.Name)));

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
    
    }
}

