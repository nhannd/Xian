#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using ClearCanvas.Workflow;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Workflow.Reporting
{
    public class Operations
    {
        public abstract class ReportingOperation
        {
            public abstract bool CanExecute(ReportingProcedureStep step, Staff currentUserStaff);
        }

        public class SaveReport : ReportingOperation
        {
            public void Execute(ReportingProcedureStep step, string reportContent, Staff supervisor, IPersistenceContext context)
            {
                if (step.ReportPart != null)
                {
                    step.ReportPart.Content = reportContent;
                    step.ReportPart.Supervisor = supervisor;
                }
                else
                {
                    Report report = new Report();
                    report.Procedure = step.RequestedProcedure;
                    ReportPart part = report.AddPart(reportContent);
                    part.Supervisor = supervisor;

                    context.Lock(report, DirtyState.New);

                    step.ReportPart = part;
                }
            }

            public override bool CanExecute(ReportingProcedureStep step, Staff currentUserStaff)
            {
                if (step.ReportPart == null || String.IsNullOrEmpty(step.ReportPart.Content))
                    return false;

                return true;
            }
        }

        public class ClaimInterpretation : ReportingOperation
        {
            public void Execute(InterpretationStep step, Staff currentUserStaff, IWorkflow workflow)
            {
                step.Assign(currentUserStaff);
            }

            public override bool CanExecute(ReportingProcedureStep step, Staff currentUserStaff)
            {
                if (step.Is<InterpretationStep>() == false)
                    return false;

                // step already started
                if (step.State != ActivityStatus.SC)
                    return false;

                // step already claim by the same staff
                if (Equals(step.AssignedStaff, currentUserStaff))
                    return false;

                return true;
            }
        }

        public class StartInterpretation : ReportingOperation
        {
            public void Execute(InterpretationStep step, Staff currentUserStaff, IWorkflow workflow)
            {
                // if not assigned, assign
                if (step.AssignedStaff == null)
                {
                    step.Assign(currentUserStaff);
                }

                if (step.State == ActivityStatus.SC)
                {
                    step.Start(currentUserStaff);
                }
            }

            public override bool CanExecute(ReportingProcedureStep step, Staff currentUserStaff)
            {
                if (step.Is<InterpretationStep>() == false)
                    return false;

                // step is completed/cancelled
                if (step.State != ActivityStatus.SC && step.State != ActivityStatus.IP)
                    return false;

                // step is not claimed or is assigned to someone else
                if (Equals(step.AssignedStaff, currentUserStaff) == false)
                    return false;

                return true;
            }
        }

        public abstract class CompleteInterpretationBase : ReportingOperation
        {
            public virtual void Execute(InterpretationStep step, Staff currentUserStaff, IWorkflow workflow)
            {
                step.Complete(currentUserStaff);
            }

            public override bool CanExecute(ReportingProcedureStep step, Staff currentUserStaff)
            {
                if (step.Is<InterpretationStep>() == false)
                    return false;

                // step is not started or has been cancelled/completed
                if (step.State != ActivityStatus.IP)
                    return false;

                // step is assigned to someone else
                if (Equals(step.AssignedStaff, currentUserStaff) == false)
                    return false;

                return true;
            }
        }

        public class CompleteInterpretationForTranscription : CompleteInterpretationBase
        {
            public new TranscriptionStep Execute(InterpretationStep step, Staff currentUserStaff, IWorkflow workflow)
            {
                base.Execute(step, currentUserStaff, workflow);

                TranscriptionStep transcription = new TranscriptionStep(step);
                workflow.AddActivity(transcription);
                return transcription;
            }
        }

        public class CompleteInterpretationForVerification : CompleteInterpretationBase
        {
            public new VerificationStep Execute(InterpretationStep step, Staff currentUserStaff, IWorkflow workflow)
            {
                base.Execute(step, currentUserStaff, workflow);

                VerificationStep verification = new VerificationStep(step);
                if (step.ReportPart.Supervisor == null)
                    verification.Assign(currentUserStaff);
                else
                    verification.Assign(step.ReportPart.Supervisor);

                workflow.AddActivity(verification);
                return verification;
            }
        }

        public class CompleteInterpretationAndVerify : CompleteInterpretationBase
        {
            public new VerificationStep Execute(InterpretationStep step, Staff currentUserStaff, IWorkflow workflow)
            {
                base.Execute(step, currentUserStaff, workflow);

                VerificationStep verification = new VerificationStep(step);
                verification.Assign(currentUserStaff);
                verification.Complete(currentUserStaff);
                workflow.AddActivity(verification);
                return verification;
            }
        }

        public class CancelReportingStep : ReportingOperation
        {
            public InterpretationStep Execute(ReportingProcedureStep step, Staff currentUserStaff, IWorkflow workflow)
            {
                step.Discontinue();

                if (step.ReportPart != null && step.ReportPart.IsAddendum)
                {
                    // if this is an addendum step, no need to create new interpretation step
                    return null;
                }
                else
                {
                    // Create a new interpretation step for a cancel step
                    InterpretationStep interpretation = new InterpretationStep(step.RequestedProcedure);

                    // TODO assign the new interpretation back to the dictating physician, from the Report object
                    //interpretation.Assign();

                    workflow.AddActivity(interpretation);
                    return interpretation;
                }
            }

            public override bool CanExecute(ReportingProcedureStep step, Staff currentUserStaff)
            {
                // step already completed or cancelled
                if (step.State == ActivityStatus.CM || step.State == ActivityStatus.DC)
                    return false;

                // cannot cancel an unclaimed interpretation step
                if (step.Is<InterpretationStep>() && step.AssignedStaff == null)
                    return false;

                return true;
            }
        }

        /// <summary>
        /// Resident are not allow to operate on Verification Step, hence they are not allowed to "Edit" a report after it has been sent "ToBeVerified"
        /// This operation cancels the existing verification step and creates a new interpretation step for resident to edit
        /// This operation can also be used by resident/radiologist alike to revise report that hasn't been started by transcriptionist
        /// </summary>
        public class ReviseReport : ReportingOperation
        {
            public InterpretationStep Execute(VerificationStep step, Staff currentUserStaff, IWorkflow workflow)
            {
                step.Discontinue();

                InterpretationStep interpretation = new InterpretationStep(step.RequestedProcedure);

                interpretation.Assign(currentUserStaff);
                interpretation.Start(currentUserStaff);

                workflow.AddActivity(interpretation);
                return interpretation;
            }

            public override bool CanExecute(ReportingProcedureStep step, Staff currentUserStaff)
            {
                if (step.Is<VerificationStep>() == false && step.Is<TranscriptionStep>() == false)
                    return false;

                // step already completed or cancelled
                if (step.State != ActivityStatus.SC)
                    return false;

                // cannot revise a report that is read by someone else
                if (Equals(step.ReportPart.Interpretor, currentUserStaff) == false)
                    return false;

                return true;
            }
        }

        public class StartVerification : ReportingOperation
        {
            public void Execute(VerificationStep step, Staff currentUserStaff, IWorkflow workflow)
            {
                // if not assigned, assign
                if (step.AssignedStaff == null)
                {
                    step.Assign(currentUserStaff);
                }

                if (step.State == ActivityStatus.SC)
                {
                    step.Start(currentUserStaff);
                }
            }

            public override bool CanExecute(ReportingProcedureStep step, Staff currentUserStaff)
            {
                if (step.Is<VerificationStep>() == false)
                    return false;

                // step is completed/cancelled
                if (step.State != ActivityStatus.SC && step.State != ActivityStatus.IP)
                    return false;

                return true;
            }
        }

        public class CompleteVerification : ReportingOperation
        {
            public void Execute(VerificationStep step, Staff currentUserStaff, IWorkflow workflow)
            {
                // this operation is legal even if the step was never started, therefore need to supply the performer
                step.Complete(currentUserStaff);
            }

            public override bool CanExecute(ReportingProcedureStep step, Staff currentUserStaff)
            {
                if (step.Is<VerificationStep>() == false)
                    return false;

                // step already completed or cancelled
                if (step.State == ActivityStatus.CM || step.State == ActivityStatus.DC)
                    return false;

                return true;
            }
        }

        public class CreateAddendum : ReportingOperation
        {
            public InterpretationStep Execute(VerificationStep step, Staff currentUserStaff, IWorkflow workflow)
            {
                InterpretationStep interpretation = new InterpretationStep(step.RequestedProcedure);
                interpretation.Assign(currentUserStaff);
                interpretation.Start(currentUserStaff);
                interpretation.ReportPart = step.ReportPart.Report.AddPart("");
                workflow.AddActivity(interpretation);
                return interpretation;
            }

            public override bool CanExecute(ReportingProcedureStep step, Staff currentUserStaff)
            {
                // can only create an addendum for a completed verification step
                if (step.Is<VerificationStep>() == false)
                    return false;

                if (step.State != ActivityStatus.CM)
                    return false;

                return true;
            }
        }
    }
}
