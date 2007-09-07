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
        public abstract void Execute(ModalityProcedureStep mps, IWorkflow workflow);
        public abstract bool CanExecute(ModalityProcedureStep mps);

        protected void UpdateCheckInStep(RequestedProcedure rp, bool procedureAborted, IWorkflow workflow)
        {
            bool allMpsComplete = rp.ModalityProcedureSteps.TrueForAll(
                delegate(ModalityProcedureStep mps) { return mps.State == ActivityStatus.CM; });

            bool allMpsTerminated = rp.ModalityProcedureSteps.TrueForAll(
                delegate(ModalityProcedureStep mps) { return mps.IsTerminated; });


            if (allMpsComplete || (allMpsTerminated && !procedureAborted))
            {
                // complete the check-in
                rp.CheckInProcedureStep.Complete();

                // schedule an interpretation
                // Note: in reality, we want to create the Interpretation step earlier (perhaps when an MPS starts)
                // so the radiologist can start interpreting as soon as the technologist finishes some part of the scan
                workflow.AddActivity(new InterpretationStep(rp));
            }
            else if (allMpsTerminated && procedureAborted)
            {
                // discontinue check-in, since procedure was aborted
                rp.CheckInProcedureStep.Discontinue();
            }
        }
    }

    public class StartModalityProcedureStepOperation : ModalityOperation
    {
        private Staff _technologist;

        public StartModalityProcedureStepOperation(Staff technologist)
        {
            _technologist = technologist;
        }

        public override void Execute(ModalityProcedureStep mps, IWorkflow workflow)
        {
            mps.Start(_technologist);
        }

        public override bool CanExecute(ModalityProcedureStep mps)
        {
            return mps.State == ActivityStatus.SC;
        }
    }

    public class CompleteModalityProcedureStepOperation : ModalityOperation
    {
        private bool _procedureAborted;

        public CompleteModalityProcedureStepOperation(bool procedureAborted)
        {
            _procedureAborted = procedureAborted;
        }

        public override void Execute(ModalityProcedureStep mps, IWorkflow workflow)
        {
            // complete the modality step
            mps.Complete();
            UpdateCheckInStep(mps.RequestedProcedure, _procedureAborted, workflow);
        }

        public override bool CanExecute(ModalityProcedureStep mps)
        {
            return mps.State == ActivityStatus.IP || mps.State == ActivityStatus.SC;
        }
    }

    public class CancelModalityProcedureStepOperation : ModalityOperation
    {
        private bool _procedureAborted;

        public CancelModalityProcedureStepOperation(bool procedureAborted)
        {
            _procedureAborted = procedureAborted;
        }

        public override void Execute(ModalityProcedureStep mps, IWorkflow workflow)
        {
            mps.Discontinue();
            UpdateCheckInStep(mps.RequestedProcedure, _procedureAborted, workflow);
        }

        public override bool CanExecute(ModalityProcedureStep mps)
        {
            return !mps.IsTerminated;
        }
    }
}
