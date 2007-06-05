using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Workflow;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Healthcare.Brokers;
using System.Collections;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare.Workflow.Reporting
{
    public class Operations
    {
        public abstract class ReportingOperation
        {
            public abstract void Execute(ReportingProcedureStep step, Staff currentUserStaff, IWorkflow workflow);
            public abstract bool CanExecute(ReportingProcedureStep step, Staff currentUserStaff);
        }

        public class ClaimInterpretation : ReportingOperation
        {
            public override void Execute(ReportingProcedureStep step, Staff currentUserStaff, IWorkflow workflow)
            {
                InterpretationStep interpretation = step as InterpretationStep;
                interpretation.Assign(currentUserStaff);
            }

            public override bool CanExecute(ReportingProcedureStep step, Staff currentUserStaff)
            {
                if (step is InterpretationStep == false)
                    return false;

                // step already started
                if (step.State != ActivityStatus.SC)
                    return false;

                // step already claim by the same staff
                if (step.AssignedStaff != null && step.AssignedStaff.Equals(currentUserStaff))
                    return false;

                return true;
            }
        }

        public class StartInterpretation : ReportingOperation
        {
            public override void Execute(ReportingProcedureStep step, Staff currentUserStaff, IWorkflow workflow)
            {
                InterpretationStep interpretation = (InterpretationStep)step;

                // if not assigned, assign
                if (interpretation.AssignedStaff == null)
                {
                    interpretation.Assign(currentUserStaff);
                }

                if (interpretation.State == ActivityStatus.SC)
                {
                    interpretation.Start(currentUserStaff);
                }
            }

            public override bool CanExecute(ReportingProcedureStep step, Staff currentUserStaff)
            {
                if (step is InterpretationStep == false)
                    return false;

                // step is completed/cancelled
                if (step.State != ActivityStatus.SC && step.State != ActivityStatus.IP)
                    return false;

                // step is not claimed or is assigned to someone else
                if (step.AssignedStaff == null || step.AssignedStaff.Equals(currentUserStaff) == false)
                    return false;

                return true;
            }
        }

        public abstract class CompleteInterpretationBase : ReportingOperation
        {
            public override void Execute(ReportingProcedureStep step, Staff currentUserStaff, IWorkflow workflow)
            {
                InterpretationStep interpretation = step as InterpretationStep;
                interpretation.Complete();
            }

            public override bool CanExecute(ReportingProcedureStep step, Staff currentUserStaff)
            {
                if (step is InterpretationStep == false)
                    return false;

                // step is not started or has been cancelled/completed
                if (step.State != ActivityStatus.IP)
                    return false;

                // step is assigned to someone else
                if (step.AssignedStaff != null && step.AssignedStaff.Equals(currentUserStaff) == false)
                    return false;

                return true;
            }
        }

        public class CompleteInterpretationForTranscription : CompleteInterpretationBase
        {
            public override void Execute(ReportingProcedureStep step, Staff currentUserStaff, IWorkflow workflow)
            {
                base.Execute(step, currentUserStaff, workflow);
                workflow.AddActivity(new TranscriptionStep(step));
            }
        }

        public class CompleteInterpretationForVerification : CompleteInterpretationBase
        {
            public override void Execute(ReportingProcedureStep step, Staff currentUserStaff, IWorkflow workflow)
            {
                base.Execute(step, currentUserStaff, workflow);

                InterpretationStep interpretation = (InterpretationStep)step;
                VerificationStep verification = new VerificationStep(interpretation);
                verification.Assign(interpretation.PerformingStaff);

                workflow.AddActivity(verification);
            }
        }

        public class CompleteInterpretationAndVerify : CompleteInterpretationBase
        {
            public override void Execute(ReportingProcedureStep step, Staff currentUserStaff, IWorkflow workflow)
            {
                base.Execute(step, currentUserStaff, workflow);

                InterpretationStep interpretation = (InterpretationStep)step;
                VerificationStep verification = new VerificationStep(interpretation);
                verification.Assign(interpretation.PerformingStaff);
                verification.Complete(interpretation.PerformingStaff);

                workflow.AddActivity(verification);
            }
        }

        public class CancelPendingTranscription : ReportingOperation
        {
            public override void Execute(ReportingProcedureStep step, Staff currentUserStaff, IWorkflow workflow)
            {
                TranscriptionStep transcription = step as TranscriptionStep;
                transcription.Discontinue();

                InterpretationStep interpretation = new InterpretationStep(transcription);

                // TODO assign the new interpretation back to the dictating physician, from the Report object
                //interpretation.Assign();

                workflow.AddActivity(interpretation);
            }

            public override bool CanExecute(ReportingProcedureStep step, Staff currentUserStaff)
            {
                if (step is TranscriptionStep == false)
                    return false;

                // step already completed or cancelled
                if (step.State == ActivityStatus.CM || step.State == ActivityStatus.DC)
                    return false;

                // step already claim by the same staff
                if (step.AssignedStaff != null && step.AssignedStaff.Equals(currentUserStaff))
                    return false;

                return true;
            }
        }

        public class StartVerification : ReportingOperation
        {
            public override void Execute(ReportingProcedureStep step, Staff currentUserStaff, IWorkflow workflow)
            {
                VerificationStep verification = step as VerificationStep;

                // if not assigned, assign
                if (verification.AssignedStaff == null)
                {
                    verification.Assign(currentUserStaff);
                }
                verification.Start(currentUserStaff);
            }

            public override bool CanExecute(ReportingProcedureStep step, Staff currentUserStaff)
            {
                if (step is VerificationStep == false)
                    return false;

                // step already started
                if (step.State != ActivityStatus.SC)
                    return false;

                // step is assigned to someone else
                if (step.AssignedStaff != null && step.AssignedStaff.Equals(currentUserStaff) == false)
                    return false;

                return true;
            }
        }

        public class CompleteVerification : ReportingOperation
        {
            public override void Execute(ReportingProcedureStep step, Staff currentUserStaff, IWorkflow workflow)
            {
                VerificationStep verification = step as VerificationStep;

                // this operation is legal even if the step was never started, therefore need to supply the performer
                verification.Complete(currentUserStaff);
            }

            public override bool CanExecute(ReportingProcedureStep step, Staff currentUserStaff)
            {
                if (step is VerificationStep == false)
                    return false;

                // step is not started or has been cancelled/completed
                if (step.State != ActivityStatus.IP)
                    return false;

                // step is assigned to someone else
                if (step.AssignedStaff != null && step.AssignedStaff.Equals(currentUserStaff) == false)
                    return false;

                return true;
            }
        }
    }
}
