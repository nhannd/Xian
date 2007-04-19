using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Workflow;
using ClearCanvas.Common;
using System.Collections;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Healthcare.Workflow.Modality
{
    public class Test
    {

    }

    [ExtensionPoint]
    public class WorkflowOperationExtensionPoint : ExtensionPoint<IOperation>
    {
    }

    public class Operations
    {
        public abstract class ModalityWorklistOperation : OperationBase
        {
            protected ModalityProcedureStep LoadStep(EntityRef stepRef, IPersistenceContext context)
            {
                return context.GetBroker<IModalityProcedureStepBroker>().Load(stepRef, EntityLoadFlags.CheckVersion);
            }
        }

        [ExtensionOf(typeof(WorkflowOperationExtensionPoint))]
        public class StartModalityProcedureStep : ModalityWorklistOperation
        {
            public override void Execute(IWorklistItem item, IList parameters, IWorkflow workflow)
            {
                ModalityProcedureStep mps = LoadStep((item as WorklistItem).RequestedProcedure, workflow.CurrentContext);
                mps.Start(this.CurrentUserStaff);
            }

            protected override bool CanExecute(IWorklistItem item)
            {
                return (item as WorklistItem).Status == ActivityStatus.SC;
            }
        }

        [ExtensionOf(typeof(WorkflowOperationExtensionPoint))]
        public class CompleteModalityProcedureStep : ModalityWorklistOperation
        {
            public override void Execute(IWorklistItem item, IList parameters, IWorkflow workflow)
            {
                ModalityProcedureStep mps = LoadStep((item as WorklistItem).RequestedProcedure, workflow.CurrentContext);
                mps.Complete(this.CurrentUserStaff);
            }

            protected override bool CanExecute(IWorklistItem item)
            {
                ActivityStatus status = (item as WorklistItem).Status;
                return status == ActivityStatus.SC || status == ActivityStatus.IP;
            }
        }

        [ExtensionOf(typeof(WorkflowOperationExtensionPoint))]
        public class CancelModalityProcedureStep : ModalityWorklistOperation
        {
            public override void Execute(IWorklistItem item, IList parameters, IWorkflow workflow)
            {
                ModalityProcedureStep mps = LoadStep((item as WorklistItem).RequestedProcedure, workflow.CurrentContext);
                mps.Discontinue();
            }

            protected override bool CanExecute(IWorklistItem item)
            {
                ActivityStatus status = (item as WorklistItem).Status;
                return status == ActivityStatus.SC || status == ActivityStatus.IP;
            }
        }
    }
}
