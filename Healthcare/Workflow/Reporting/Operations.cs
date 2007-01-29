using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Workflow.Reporting
{
    public class Operations
    {
        public class ClaimInterpretation : Operation
        {
            protected override void Execute(ProcedureStep step, IWorkflow workflow)
            {
                InterpretationStep interpretation = (InterpretationStep)step;
                interpretation.Assign(this.CurrentUserStaff);
            }
        }

        public class StartInterpretation : Operation
        {
            protected override void Execute(ProcedureStep step, IWorkflow workflow)
            {
                InterpretationStep interpretation = (InterpretationStep)step;

                // if not assigned, assign
                if (interpretation.AssignedStaff == null)
                {
                    interpretation.Assign(this.CurrentUserStaff);
                }
                interpretation.Start(this.CurrentUserStaff);
            }
        }

        public abstract class CompleteInterpretationBase : Operation
        {
            protected override void Execute(ProcedureStep step, IWorkflow workflow)
            {
                InterpretationStep interpretation = (InterpretationStep)step;
                interpretation.Complete();
            }
        }

        public class CompleteInterpretationForTranscription : CompleteInterpretationBase
        {
            protected override void Execute(ProcedureStep step, IWorkflow workflow)
            {
                base.Execute(step, workflow);

                workflow.AddActivity(new TranscriptionStep((InterpretationStep)step));
            }
        }

        public class CompleteInterpretationForVerification : CompleteInterpretationBase
        {
            protected override void Execute(ProcedureStep step, IWorkflow workflow)
            {
                base.Execute(step, workflow);

                InterpretationStep interpretation = (InterpretationStep)step;
                VerificationStep verification = new VerificationStep(interpretation);
                verification.Assign(interpretation.PerformingStaff);

                workflow.AddActivity(verification);
            }
        }

        public class CompleteInterpretationAndVerify : CompleteInterpretationBase
        {
            protected override void Execute(ProcedureStep step, IWorkflow workflow)
            {
                base.Execute(step, workflow);

                InterpretationStep interpretation = (InterpretationStep)step;
                VerificationStep verification = new VerificationStep(interpretation);
                verification.Assign(interpretation.PerformingStaff);
                verification.Complete(interpretation.PerformingStaff);

                workflow.AddActivity(verification);
            }
        }

        public class CancelPendingTranscription : Operation
        {
            protected override void Execute(ProcedureStep step, IWorkflow workflow)
            {
                TranscriptionStep transcription = (TranscriptionStep)step;
                transcription.Discontinue();

                InterpretationStep interpretation = new InterpretationStep(transcription);

                // TODO assign the new interpretation back to the dictating physician, from the Report object
                //interpretation.Assign();

                workflow.AddActivity(interpretation);
            }
        }

        public class StartVerification : Operation
        {
            protected override void Execute(ProcedureStep step, IWorkflow workflow)
            {
                VerificationStep verification = (VerificationStep)step;

                // if not assigned, assign
                if (verification.AssignedStaff == null)
                {
                    verification.Assign(this.CurrentUserStaff);
                }
                verification.Start(this.CurrentUserStaff);
            }
        }

        public class CompleteVerification : Operation
        {
            protected override void Execute(ProcedureStep step, IWorkflow workflow)
            {
                VerificationStep verification = (VerificationStep)step;

                // this operation is legal even if the step was never started, therefore need to supply the performer
                verification.Complete(this.CurrentUserStaff);
            }
        }
    }
}
