using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Workflow;
using ClearCanvas.Common;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare.Workflow
{
    [ExtensionPoint]
    public class WorkflowOperationExtensionPoint : ExtensionPoint<IOperation>
    {
    }

    public class Operations
    {
        [ExtensionOf(typeof(WorkflowOperationExtensionPoint))]
        public class CheckIn : Operation
        {
            protected override void Execute(ProcedureStep step, IWorkflow workflow)
            {
                //ModalityProcedureStep mps = (ModalityProcedureStep)step;
                //mps.Start(this.CurrentUserStaff);
                Platform.ShowMessageBox("Check-in Not Implemented");
            }

            protected override bool CanExecute(ProcedureStep step)
            {
                ModalityProcedureStep mps = (ModalityProcedureStep)step;
                return mps.State == ActivityStatus.SC;
            }
        }

        [ExtensionOf(typeof(WorkflowOperationExtensionPoint))]
        public class Cancel : Operation
        {
            protected override void Execute(ProcedureStep step, IWorkflow workflow)
            {
                //ModalityProcedureStep mps = (ModalityProcedureStep)step;
                //mps.Start(this.CurrentUserStaff);
                Platform.ShowMessageBox("Cancel Not Implemented");
            }

            protected override bool CanExecute(ProcedureStep step)
            {
                ModalityProcedureStep mps = (ModalityProcedureStep)step;
                return (mps.State == ActivityStatus.IP
                    || mps.State == ActivityStatus.SC
                    || mps.State == ActivityStatus.SU);
            }
        }
    
    }
}
