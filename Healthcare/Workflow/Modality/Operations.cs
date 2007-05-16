using System;
using System.Collections;
using System.Collections.Generic;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Workflow.Modality
{
    public abstract class ModalityOperation
    {
    }

    public class StartModalityProcedureStepOperation : ModalityOperation
    {
        public void Execute(ModalityProcedureStep mps, Staff currentUserStaff, IWorkflow workflow)
        {
            mps.Start(currentUserStaff);
        }

        public bool CanExecute(ModalityProcedureStep mps)
        {
            return mps.State == ActivityStatus.SC;
        }
    }

    public class CompleteModalityProcedureStepOperation : ModalityOperation
    {
        public void Execute(ModalityProcedureStep mps, Staff currentUserStaff, IWorkflow workflow)
        {
            mps.Complete(currentUserStaff);
        }

        public bool CanExecute(ModalityProcedureStep mps)
        {
            return mps.State == ActivityStatus.SC || mps.State == ActivityStatus.IP;
        }
    }

    public class CancelModalityProcedureStepOperation : ModalityOperation
    {
        public void Execute(ModalityProcedureStep mps, Staff currentUserStaff, IWorkflow workflow)
        {
            mps.Discontinue();
        }

        public bool CanExecute(ModalityProcedureStep mps)
        {
            return mps.State == ActivityStatus.SC || mps.State == ActivityStatus.IP;
        }
    }

    public class SuspendModalityProcedureStepOperation : ModalityOperation
    {
        public void Execute(ModalityProcedureStep mps, Staff currentUserStaff, IWorkflow workflow)
        {
            mps.Suspend();
        }

        public bool CanExecute(ModalityProcedureStep mps)
        {
            return mps.State == ActivityStatus.SC || mps.State == ActivityStatus.IP;
        }
    }
}
