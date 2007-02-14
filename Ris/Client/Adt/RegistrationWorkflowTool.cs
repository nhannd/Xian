using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Ris.Client.Adt
{
    public class RegistrationWorkflowTool
    {
        public abstract class WorkflowItemTool : Tool<IRegistrationWorkflowItemToolContext>
        {
            private string _operationClass;

            public WorkflowItemTool(string operationClass)
            {
                _operationClass = operationClass;
            }

            public virtual bool Enabled
            {
                get
                {
                    return this.Context.GetWorkflowOperationEnablement(_operationClass);
                }
            }

            public virtual event EventHandler EnabledChanged
            {
                add { this.Context.SelectedItemsChanged += value; }
                remove { this.Context.SelectedItemsChanged -= value; }
            }

            public virtual void Apply()
            {
                this.Context.ExecuteWorkflowOperation(_operationClass);
            }
        }

        [MenuAction("apply", "folderexplorer-items-contextmenu/Check-in")]
        [ClickHandler("apply", "Apply")]
        [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
        [ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
        public class StartTool : WorkflowItemTool
        {
            public StartTool()
                : base("ClearCanvas.Healthcare.Workflow.Registration.Operations+CheckIn")
            {
            }
        }

        [MenuAction("apply", "folderexplorer-items-contextmenu/Cancel")]
        [ClickHandler("apply", "Apply")]
        [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
        [ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
        public class CancelTool : WorkflowItemTool
        {
            public CancelTool()
                : base("ClearCanvas.Healthcare.Workflow.Registration.Operations+Cancel")
            {
            }
        }
    
    }
}

