using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Workflow;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare.Workflow.Registration
{
    [ExtensionPoint]
    public class WorkflowOperationExtensionPoint : ExtensionPoint<IOperation>
    {
    }

    public class Operations
    {
        [ExtensionOf(typeof(WorkflowOperationExtensionPoint))]
        public class CheckInPatient : Operation
        {
            protected override void Execute(ProcedureStep step, IWorkflow workflow)
            {
                //ModalityProcedureStep mps = (ModalityProcedureStep)step;
                //mps.Start(this.CurrentUserStaff);
                Platform.ShowMessageBox("Not Implemented");
            }

            protected override bool CanExecute(ProcedureStep step)
            {
                ModalityProcedureStep mps = (ModalityProcedureStep)step;
                return mps.State == ActivityStatus.SC;
            }
        }
    }
}
