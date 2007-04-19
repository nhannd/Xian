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
        public abstract class ReportingOperation : OperationBase
        {
            protected ReportingProcedureStep LoadStep(EntityRef stepRef, IPersistenceContext context)
            {
                // it is extremly important that we get the actual object and not a proxy here
                // if a proxy is returned, then it cannot be cast to a subclass
                // (eg InterpretationStep s = (InterpretationStep)rps; will fail even if we know that rps is an interpretation step)
                return context.GetBroker<IReportingProcedureStepBroker>().Load(stepRef, EntityLoadFlags.CheckVersion);
            }
        }

        public class ClaimInterpretation : ReportingOperation
        {
            public override void Execute(IWorklistItem item, IList parameters, IWorkflow workflow)
            {
                InterpretationStep interpretation = (InterpretationStep)LoadStep((item as WorklistItem).ProcedureStep, workflow.CurrentContext);
                interpretation.Assign(this.CurrentUserStaff);
            }
        }

        public class StartInterpretation : ReportingOperation
        {
            public override void Execute(IWorklistItem item, IList parameters, IWorkflow workflow)
            {
                InterpretationStep interpretation = (InterpretationStep)LoadStep((item as WorklistItem).ProcedureStep, workflow.CurrentContext);

                // if not assigned, assign
                if (interpretation.AssignedStaff == null)
                {
                    interpretation.Assign(this.CurrentUserStaff);
                }
                interpretation.Start(this.CurrentUserStaff);
            }
        }

        public abstract class CompleteInterpretationBase : ReportingOperation
        {
            public virtual void Execute(ReportingProcedureStep step, IWorkflow workflow)
            {
                InterpretationStep interpretation = (InterpretationStep)step;
                interpretation.Complete();
            }
        }

        public class CompleteInterpretationForTranscription : CompleteInterpretationBase
        {
            public override void Execute(IWorklistItem item, IList parameters, IWorkflow workflow)
            {
                ReportingProcedureStep step = LoadStep((item as WorklistItem).ProcedureStep, workflow.CurrentContext);
                base.Execute(step, workflow);
                workflow.AddActivity(new TranscriptionStep(step));
            }
        }

        public class CompleteInterpretationForVerification : CompleteInterpretationBase
        {
            public override void Execute(IWorklistItem item, IList parameters, IWorkflow workflow)
            {
                ReportingProcedureStep step = LoadStep((item as WorklistItem).ProcedureStep, workflow.CurrentContext);
                base.Execute(step, workflow);

                InterpretationStep interpretation = (InterpretationStep)step;
                VerificationStep verification = new VerificationStep(interpretation);
                verification.Assign(interpretation.PerformingStaff);

                workflow.AddActivity(verification);
            }
        }

        public class CompleteInterpretationAndVerify : CompleteInterpretationBase
        {
            public override void Execute(IWorklistItem item, IList parameters, IWorkflow workflow)
            {
                ReportingProcedureStep step = LoadStep((item as WorklistItem).ProcedureStep, workflow.CurrentContext);
                base.Execute(step, workflow);

                InterpretationStep interpretation = (InterpretationStep)step;
                VerificationStep verification = new VerificationStep(interpretation);
                verification.Assign(interpretation.PerformingStaff);
                verification.Complete(interpretation.PerformingStaff);

                workflow.AddActivity(verification);
            }
        }

        public class CancelPendingTranscription : ReportingOperation
        {
            public override void Execute(IWorklistItem item, IList parameters, IWorkflow workflow)
            {
                TranscriptionStep transcription = (TranscriptionStep)LoadStep((item as WorklistItem).ProcedureStep, workflow.CurrentContext);
                transcription.Discontinue();

                InterpretationStep interpretation = new InterpretationStep(transcription);

                // TODO assign the new interpretation back to the dictating physician, from the Report object
                //interpretation.Assign();

                workflow.AddActivity(interpretation);
            }
        }

        public class StartVerification : ReportingOperation
        {
            public override void Execute(IWorklistItem item, IList parameters, IWorkflow workflow)
            {
                VerificationStep verification = (VerificationStep)LoadStep((item as WorklistItem).ProcedureStep, workflow.CurrentContext);

                // if not assigned, assign
                if (verification.AssignedStaff == null)
                {
                    verification.Assign(this.CurrentUserStaff);
                }
                verification.Start(this.CurrentUserStaff);
            }
        }

        public class CompleteVerification : ReportingOperation
        {
            public override void Execute(IWorklistItem item, IList parameters, IWorkflow workflow)
            {
                VerificationStep verification = (VerificationStep)LoadStep((item as WorklistItem).ProcedureStep, workflow.CurrentContext);

                // this operation is legal even if the step was never started, therefore need to supply the performer
                verification.Complete(this.CurrentUserStaff);
            }
        }
    }
}
