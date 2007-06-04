using System;
using System.Collections;
using System.Collections.Generic;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Workflow;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Workflow.Modality
{
    public abstract class ModalityOperation
    {
        public abstract void Execute(ModalityProcedureStep mps, Staff currentUserStaff, IWorkflow workflow);
        public abstract bool CanExecute(ModalityProcedureStep mps);
    }

    public class StartModalityProcedureStepOperation : ModalityOperation
    {
        public override void Execute(ModalityProcedureStep mps, Staff currentUserStaff, IWorkflow workflow)
        {
            mps.Start(currentUserStaff);

            if (mps.RequestedProcedure.Status == ActivityStatus.SC)
                mps.RequestedProcedure.Start(currentUserStaff);
        }

        public override bool CanExecute(ModalityProcedureStep mps)
        {
            return mps.State == ActivityStatus.SC;
        }
    }

    public class CompleteModalityProcedureStepOperation : ModalityOperation
    {
        public override void Execute(ModalityProcedureStep mps, Staff currentUserStaff, IWorkflow workflow)
        {
            if (mps.State == ActivityStatus.IP)
            {
                mps.Complete();
            }
            else if (mps.State == ActivityStatus.SC)
            {
                mps.Complete(currentUserStaff);
            }

            if (mps.RequestedProcedure.IsAllMpsCompletedOrDiscontinued)
            {
                mps.RequestedProcedure.Complete(currentUserStaff);

                // Note: this is a simplify model.  In reality, we want to create the Interpretation step earlier (perhaps when an MPS starts)
                // so the radiologist can starts interpreting as soon as the technologist finish some part of the scan
                workflow.AddActivity(new InterpretationStep(mps.RequestedProcedure));
            }
        }

        public override bool CanExecute(ModalityProcedureStep mps)
        {
            return mps.State == ActivityStatus.IP || mps.State == ActivityStatus.SC;
        }
    }

    public class CancelModalityProcedureStepOperation : ModalityOperation
    {
        public override void Execute(ModalityProcedureStep mps, Staff currentUserStaff, IWorkflow workflow)
        {
            mps.Discontinue();

            if (mps.RequestedProcedure.IsAllMpsDiscontinued)
            {
                mps.RequestedProcedure.Discontinue();
            }
            else if (mps.RequestedProcedure.IsAllMpsCompletedOrDiscontinued)
            {
                mps.RequestedProcedure.Complete(currentUserStaff);
            }
        }

        public override bool CanExecute(ModalityProcedureStep mps)
        {
            return mps.State == ActivityStatus.SC || mps.State == ActivityStatus.IP || mps.State == ActivityStatus.SU;
        }
    }

    public class SuspendModalityProcedureStepOperation : ModalityOperation
    {
        public override void Execute(ModalityProcedureStep mps, Staff currentUserStaff, IWorkflow workflow)
        {
            mps.Suspend();
        }

        public override bool CanExecute(ModalityProcedureStep mps)
        {
            return mps.State == ActivityStatus.IP;
        }
    }

    public class ResumeModalityProcedureStepOperation : ModalityOperation
    {
        public override void Execute(ModalityProcedureStep mps, Staff currentUserStaff, IWorkflow workflow)
        {
            mps.Resume();
        }

        public override bool CanExecute(ModalityProcedureStep mps)
        {
            return mps.State == ActivityStatus.SU;
        }
    }
}
