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
        public abstract class WorkflowItemTool : Tool<IRegistrationWorkflowItemToolContext>
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

            public abstract void Apply();
        }

        [MenuAction("apply", "folderexplorer-items-contextmenu/Check-in")]
        [ClickHandler("apply", "Apply")]
        [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
        [ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
        public class CheckInTool : WorkflowItemTool
        {
            public CheckInTool()
                : base("CheckInProcedure")
            {
            }

            public override void Apply()
            {
                foreach (RegistrationWorklistItem item in this.Context.SelectedItems)
                {
                    RequestedProcedureCheckInComponent checkInComponent = new RequestedProcedureCheckInComponent(item);
                    ApplicationComponent.LaunchAsDialog(
                        this.Context.DesktopWindow, checkInComponent, String.Format("Checking in {0}", Format.Custom(item.Name)));
                }
            }
        }

        [MenuAction("apply", "folderexplorer-items-contextmenu/Cancel")]
        [ClickHandler("apply", "Apply")]
        [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
        [ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
        public class CancelTool : WorkflowItemTool
        {
            public CancelTool()
                : base("CancelOrder")
            {
            }

            public override void Apply()
            {
                foreach (RegistrationWorklistItem item in this.Context.SelectedItems)
                {
                    CancelOrderComponent cancelOrderComponent = new CancelOrderComponent(item.PatientProfileRef);
                    ApplicationComponent.LaunchAsDialog(
                        this.Context.DesktopWindow, cancelOrderComponent, String.Format("Cancel Order for {0}", Format.Custom(item.Name)));
                }
            }
        }
    
    }
}

