using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Workflow;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare.Workflow.Modality
{
    [ExtensionPoint]
    public class WorkflowOperationExtensionPoint : ExtensionPoint<IOperation>
    {
    }

    public class Operations
    {
        [ExtensionOf(typeof(WorkflowOperationExtensionPoint))]
        public class StartModalityProcedureStep : Operation
        {
            protected override void Execute(ProcedureStep step, IWorkflow workflow)
            {
                ModalityProcedureStep mps = (ModalityProcedureStep)step;
                mps.Start(this.CurrentUserStaff);
            }

            protected override bool CanExecute(ProcedureStep step)
            {
                ModalityProcedureStep mps = (ModalityProcedureStep)step;
                return mps.State == ActivityStatus.SC;
            }
        }

        [ExtensionOf(typeof(WorkflowOperationExtensionPoint))]
        public class CompleteModalityProcedureStep : Operation
        {
            protected override void Execute(ProcedureStep step, IWorkflow workflow)
            {
                ModalityProcedureStep mps = (ModalityProcedureStep)step;
                mps.Complete(this.CurrentUserStaff);
            }

            protected override bool CanExecute(ProcedureStep step)
            {
                ModalityProcedureStep mps = (ModalityProcedureStep)step;
                return mps.State == ActivityStatus.SC || mps.State == ActivityStatus.IP;
            }
        }

        [ExtensionOf(typeof(WorkflowOperationExtensionPoint))]
        public class CancelModalityProcedureStep : Operation
        {
            protected override void Execute(ProcedureStep step, IWorkflow workflow)
            {
                ModalityProcedureStep mps = (ModalityProcedureStep)step;
                mps.Discontinue();
            }

            protected override bool CanExecute(ProcedureStep step)
            {
                ModalityProcedureStep mps = (ModalityProcedureStep)step;
                return mps.State == ActivityStatus.SC || mps.State == ActivityStatus.IP;
            }
        }
    }
}
