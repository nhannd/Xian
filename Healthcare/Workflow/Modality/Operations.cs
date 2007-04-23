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
    public class Operations
    {
        public abstract class ModalityOperation
        {
            protected ModalityProcedureStep LoadStep(EntityRef stepRef, IPersistenceContext context)
            {
                return context.GetBroker<IModalityProcedureStepBroker>().Load(stepRef, EntityLoadFlags.CheckVersion);
            }
        }

        public class StartModalityProcedureStep : ModalityOperation
        {
            public void Execute(EntityRef rpRef, Staff currentUserStaff, IWorkflow workflow)
            {
                ModalityProcedureStep mps = LoadStep(rpRef, workflow.CurrentContext);
                mps.Start(currentUserStaff);
            }

            public bool CanExecute(ModalityProcedureStep step)
            {
                return step.State == ActivityStatus.SC;
            }
        }

        public class CompleteModalityProcedureStep : ModalityOperation
        {
            public void Execute(EntityRef rpRef, Staff currentUserStaff, IWorkflow workflow)
            {
                ModalityProcedureStep mps = LoadStep(rpRef, workflow.CurrentContext);
                mps.Complete(currentUserStaff);
            }

            public bool CanExecute(ModalityProcedureStep step)
            {
                return step.State == ActivityStatus.SC || step.State == ActivityStatus.IP;
            }
        }

        public class CancelModalityProcedureStep : ModalityOperation
        {
            public void Execute(EntityRef rpRef, Staff currentUserStaff, IWorkflow workflow)
            {
                ModalityProcedureStep mps = LoadStep(rpRef, workflow.CurrentContext);
                mps.Discontinue();
            }

            public bool CanExecute(ModalityProcedureStep step)
            {
                return step.State == ActivityStatus.SC || step.State == ActivityStatus.IP;
            }
        }
    }
}
