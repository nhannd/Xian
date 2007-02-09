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
        public abstract class WorkflowTool : Tool<IRegistrationWorkflowToolContext>
        {
            private string _operationClass;

            public WorkflowTool(string operationClass)
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
        [ExtensionOf(typeof(RegistrationWorkflowToolExtensionPoint))]
        public class StartTool : WorkflowTool
        {
            public StartTool()
                : base("ClearCanvas.Healthcare.Workflow.Registration.Operations+CheckInPatient")
            {
            }
        }
    }
}

