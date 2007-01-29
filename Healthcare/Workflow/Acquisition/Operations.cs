using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Workflow.Acquisition
{
    public class Operations
    {
        public class StartModalityProcedureStep : Operation
        {
            protected override void Execute(ProcedureStep step, IWorkflow workflow)
            {
                ModalityProcedureStep mps = (ModalityProcedureStep)step;
                mps.Start(this.CurrentUserStaff);
            }
        }

        public class CompleteModalityProcedureStep : Operation
        {
            protected override void Execute(ProcedureStep step, IWorkflow workflow)
            {
                ModalityProcedureStep mps = (ModalityProcedureStep)step;
                mps.Complete(this.CurrentUserStaff);
            }
        }

        public class CancelModalityProcedureStep : Operation
        {
            protected override void Execute(ProcedureStep step, IWorkflow workflow)
            {
                ModalityProcedureStep mps = (ModalityProcedureStep)step;
                mps.Discontinue();
            }
        }
    }
}
