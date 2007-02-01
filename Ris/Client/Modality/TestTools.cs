using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Ris.Client.Modality
{
    public class TestTools
    {
        public abstract class WorkflowTool : Tool<IModalityWorkflowToolContext>
        {
            private string _operationClass;

            public WorkflowTool(string operationClass)
            {
                _operationClass = operationClass;
            }

            public bool Enabled
            {
                get
                {
                    return this.Context.GetWorkflowOperationEnablement(_operationClass);
                }
            }

            public event EventHandler EnabledChanged
            {
                add { this.Context.SelectedItemsChanged += value; }
                remove { this.Context.SelectedItemsChanged -= value; }
            }

            public void Apply()
            {
                this.Context.ExecuteWorkflowOperation(_operationClass);
            }
        }

        [MenuAction("apply", "folderexplorer-items-contextmenu/Start")]
        [ClickHandler("apply", "Apply")]
        [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
        [ExtensionOf(typeof(ModalityWorkflowToolExtensionPoint))]
        public class StartTool : WorkflowTool
        {
            public StartTool()
                :base("ClearCanvas.Healthcare.Workflow.Modality.Operations+StartModalityProcedureStep")
            {
            }
        }

        [MenuAction("apply", "folderexplorer-items-contextmenu/Complete")]
        [ClickHandler("apply", "Apply")]
        [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
        [ExtensionOf(typeof(ModalityWorkflowToolExtensionPoint))]
        public class CompleteTool : WorkflowTool
        {
            public CompleteTool()
                : base("ClearCanvas.Healthcare.Workflow.Modality.Operations+CompleteModalityProcedureStep")
            {
            }
        }

        [MenuAction("apply", "folderexplorer-items-contextmenu/Cancel")]
        [ClickHandler("apply", "Apply")]
        [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
        [ExtensionOf(typeof(ModalityWorkflowToolExtensionPoint))]
        public class CancelTool : WorkflowTool
        {
            public CancelTool()
                : base("ClearCanvas.Healthcare.Workflow.Modality.Operations+CancelModalityProcedureStep")
            {
            }
        }


    }
}
