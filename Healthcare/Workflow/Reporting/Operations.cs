#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Workflow;
using Iesi.Collections.Generic;

namespace ClearCanvas.Healthcare.Workflow.Reporting
{
	public class Operations
	{
		public abstract class ReportingOperation
		{
			public abstract bool CanExecute(ReportingProcedureStep step, Staff executingStaff);

			protected PublicationStep CreateScheduledPublicationStep(Staff executingStaff, VerificationStep verification)
			{
				ReportingWorkflowSettings settings = new ReportingWorkflowSettings();

				PublicationStep publication = new PublicationStep(verification);
				publication.Assign(executingStaff);
				publication.Schedule(Platform.Time.AddSeconds(settings.PublicationDelay));

				return publication;
			}
		}

		public class SaveReport : ReportingOperation
		{
			public void Execute(ReportingProcedureStep step, Dictionary<string, string> reportPartExtendedProperties, Staff supervisor, IPersistenceContext context)
			{
				step.ReportPart.Supervisor = supervisor;
				foreach (KeyValuePair<string, string> pair in reportPartExtendedProperties)
				{
					step.ReportPart.ExtendedProperties[pair.Key] = pair.Value;
				}
			}

			public override bool CanExecute(ReportingProcedureStep step, Staff executingStaff)
			{
				// cannot save if no report part
				if (step.ReportPart == null)
					return false;

				// cannot save if the report part is no longer modifiable
				if (!step.ReportPart.IsModifiable)
					return false;

				// only the owner of this step can save
				if (step.PerformingStaff != null && !Equals(step.PerformingStaff, executingStaff))
					return false;

				return true;
			}
		}

		public class StartInterpretation : ReportingOperation
		{
			public void Execute(InterpretationStep step, Staff executingStaff, List<InterpretationStep> linkInterpretations, IWorkflow workflow)
			{
				// if not assigned, assign
				if (step.AssignedStaff == null)
				{
					step.Assign(executingStaff);
				}

				// put in-progress
				step.Start(executingStaff);

				// if a report has not yet been created for this step, create now
				if (step.ReportPart == null)
				{
					Report report = new Report(step.Procedure);
					ReportPart part = report.ActivePart;

					workflow.CurrentContext.Lock(report, DirtyState.New);

					step.ReportPart = part;
				}

				// attach linked interpretations to this report
				foreach (InterpretationStep interpretation in linkInterpretations)
				{
					interpretation.LinkToReport(step.ReportPart.Report);
				}
			}

			public override bool CanExecute(ReportingProcedureStep step, Staff executingStaff)
			{
				// must be an interpretation step
				if (step.Is<InterpretationStep>() == false)
					return false;

				// must be scheduled
				if (step.State != ActivityStatus.SC)
					return false;

				// must not be assigned to another staff
				if (step.AssignedStaff != null && !Equals(step.AssignedStaff, executingStaff))
					return false;

				return true;
			}
		}

		public class StartTranscriptionReview : ReportingOperation
		{
			public void Execute(TranscriptionReviewStep step, Staff executingStaff)
			{
				step.Start(executingStaff);
			}

			public override bool CanExecute(ReportingProcedureStep step, Staff executingStaff)
			{
				// must be an interpretation step
				if (step.Is<TranscriptionReviewStep>() == false)
					return false;

				// must be scheduled
				if (step.State != ActivityStatus.SC)
					return false;

				// must not be assigned to another staff
				if (!Equals(step.AssignedStaff, executingStaff))
					return false;

				return true;
			}
		}

		public abstract class CompleteInterpretationBase : ReportingOperation
		{
			protected void UpdateStep(ReportingProcedureStep step, Staff executingStaff)
			{
				if (step.PerformingStaff == null)
					step.Complete(executingStaff);
				else
					step.Complete();

				// move draft report to prelim status
				if (step.ReportPart.Status == ReportPartStatus.D)
					step.ReportPart.MarkPreliminary();
			}

			public override bool CanExecute(ReportingProcedureStep step, Staff executingStaff)
			{
				if (!step.Is<InterpretationStep>() && !step.Is<TranscriptionReviewStep>())
					return false;

				// interpretation steps must be in progress
				if (step.Is<InterpretationStep>() && step.State != ActivityStatus.IP)
					return false;

				// transcription review step must not be terminated
				if (step.Is<TranscriptionReviewStep>() && step.IsTerminated)
					return false;

				// must not be assigned to someone else
				if (!Equals(step.AssignedStaff, executingStaff))
					return false;

				return true;
			}
		}

		public class CompleteInterpretationForTranscription : CompleteInterpretationBase
		{
			public ReportingProcedureStep Execute(ReportingProcedureStep step, Staff executingStaff, IWorkflow workflow)
			{
				UpdateStep(step, executingStaff);

				TranscriptionStep transcription = new TranscriptionStep(step);
				workflow.AddActivity(transcription);
				return transcription;
			}
		}

		public class CompleteInterpretationForVerification : CompleteInterpretationBase
		{
			public ReportingProcedureStep Execute(ReportingProcedureStep step, Staff executingStaff, IWorkflow workflow)
			{
				UpdateStep(step, executingStaff);

				VerificationStep verification = new VerificationStep(step);

				// supervisor can be null, in which case the verification step is unassigned.
				verification.Assign(step.ReportPart.Supervisor);

				workflow.AddActivity(verification);
				return verification;
			}
		}

		public class CompleteInterpretationAndVerify : CompleteInterpretationBase
		{
			public ReportingProcedureStep Execute(ReportingProcedureStep step, Staff executingStaff, IWorkflow workflow)
			{
				UpdateStep(step, executingStaff);

				VerificationStep verification = new VerificationStep(step);
				verification.Assign(executingStaff);
				verification.Complete(executingStaff);
				workflow.AddActivity(verification);

				PublicationStep publication = CreateScheduledPublicationStep(executingStaff, verification);
				workflow.AddActivity(publication);

				return publication;
			}
		}

		public class CancelReportingStep : ReportingOperation
		{
			public List<InterpretationStep> Execute(ReportingProcedureStep step, Staff executingStaff, Staff assignStaff, IWorkflow workflow)
			{
				step.Discontinue();

				// cancel the report part if exists
				if (step.ReportPart != null)
					step.ReportPart.Cancel();

				List<InterpretationStep> interpretations = new List<InterpretationStep>();
				if (!IsAddendumStep(step))
				{
					HashedSet<Procedure> procedures = new HashedSet<Procedure>();
					procedures.Add(step.Procedure);

					// if there are linked procedures, schedule a new interpretation for each procedure being reported
					if (step.ReportPart != null)
					{
						procedures.AddAll(step.ReportPart.Report.Procedures);
					}

					// schedule new interpretations
					foreach (Procedure procedure in procedures)
					{
						InterpretationStep interpretation = new InterpretationStep(procedure);
						interpretation.Schedule(Platform.Time);
						if (assignStaff != null)
							interpretation.Assign(assignStaff);

						interpretations.Add(interpretation);
						workflow.AddActivity(interpretation);
					}
				}
				return interpretations;
			}

			public override bool CanExecute(ReportingProcedureStep step, Staff executingStaff)
			{
				// Publication steps cannot be cancelled after starting
				if (step.Is<PublicationStep>() && step.State != ActivityStatus.SC)
					return false;

				// cannot cancel an unclaimed interpretation step
				if (step.Is<InterpretationStep>() && step.AssignedStaff == null)
					return false;

				// cannot cancel once transcription has begun
				if (step.Is<TranscriptionStep>() && step.State != ActivityStatus.SC)
					return false;

				// cannot cancel a step that is already completed or cancelled
				if (step.IsTerminated)
					return false;

				return true;
			}

			private bool IsAddendumStep(ReportingProcedureStep step)
			{
				return step.ReportPart != null && step.ReportPart.IsAddendum;
			}
		}

		/// <summary>
		/// Residents are not allowed to edit a report that has been scheduled for verification.
		/// This operation cancels the scheduled verification and creates a new interpretation step for the resident to edit.
		/// </summary>
		public class ReviseResidentReport : ReportingOperation
		{
			public InterpretationStep Execute(VerificationStep step, Staff executingStaff, IWorkflow workflow)
			{
				// Cancel the current step
				step.Discontinue();

				// Create a new interpreatation step that uses the same report part
				InterpretationStep interpretation = new InterpretationStep(step);

				// Reset the interpretator
				interpretation.ReportPart.Interpreter = null;

				// Assign the new step to the resident
				interpretation.Assign(executingStaff);
				interpretation.Start(executingStaff);

				workflow.AddActivity(interpretation);
				return interpretation;
			}

			public override bool CanExecute(ReportingProcedureStep step, Staff executingStaff)
			{
				if (step.Is<VerificationStep>() == false)
					return false;

				// must be scheduled
				if (step.State != ActivityStatus.SC)
					return false;

				// cannot revise a report that was read by someone else
				if (!Equals(step.ReportPart.Interpreter, executingStaff))
					return false;

				return true;
			}
		}

		public class StartVerification : ReportingOperation
		{
			public void Execute(VerificationStep step, Staff executingStaff, IWorkflow workflow)
			{
				// if not assigned, assign
				if (step.AssignedStaff == null)
				{
					step.Assign(executingStaff);
				}

				// put in-progress
				step.Start(executingStaff);
			}

			public override bool CanExecute(ReportingProcedureStep step, Staff executingStaff)
			{
				if (!step.Is<VerificationStep>())
					return false;

				// must be scheduled
				if (step.State != ActivityStatus.SC)
					return false;

				if (step.AssignedStaff != null && !Equals(executingStaff, step.AssignedStaff))
					return false;

				if (step.ReportPart.Supervisor != null && !Equals(executingStaff, step.ReportPart.Supervisor))
					return false;

				return true;
			}
		}

		public class CompleteVerification : ReportingOperation
		{
			public PublicationStep Execute(VerificationStep step, Staff executingStaff, IWorkflow workflow)
			{
				// this operation is legal even if the step was never started, therefore need to supply the performer
				step.Complete(executingStaff);

				PublicationStep publication = CreateScheduledPublicationStep(executingStaff, step);
				workflow.AddActivity(publication);

				return publication;
			}

			public override bool CanExecute(ReportingProcedureStep step, Staff executingStaff)
			{
				if (!step.Is<VerificationStep>())
					return false;

				// must not be already completed or cancelled
				if (step.IsTerminated)
					return false;

				if (step.AssignedStaff != null && !Equals(executingStaff, step.AssignedStaff))
					return false;

				if (step.ReportPart.Supervisor != null && !Equals(executingStaff, step.ReportPart.Supervisor))
					return false;

				return true;
			}
		}

		public class CreateAddendum : ReportingOperation
		{
			public InterpretationStep Execute(Procedure procedure, Staff executingStaff, IWorkflow workflow)
			{
				// the procedure passed in may be any one of the procedures that this report covers
				// ideally, the new interpretation step should be created on the procedure that the
				// publication step was linked to (and only one of the reported procedures should have a publication step)
				procedure = CollectionUtils.SelectFirst(procedure.ActiveReport.Procedures,
						delegate(Procedure p)
						{
							return CollectionUtils.Contains(p.ReportingProcedureSteps,
															delegate(ReportingProcedureStep ps) { return ps.Is<PublicationStep>() && ps.State == ActivityStatus.CM; });
						})
					// but if there are no publication steps (i.e. imported data), then just use the procedure that was provided.
					// See bug #3450
					?? procedure;

				InterpretationStep interpretation = new InterpretationStep(procedure);
				interpretation.Assign(executingStaff);
				interpretation.ReportPart = procedure.ActiveReport.AddAddendum();
				workflow.AddActivity(interpretation);
				return interpretation;
			}

			public bool CanExecute(Procedure procedure, Staff executingStaff)
			{
				// a procedure must have an active report in order to create an addendum
				if (procedure.ActiveReport == null)
					return false;

				// cannot add a new addendum while there is still an active part
				if (procedure.ActiveReport.ActivePart != null)
					return false;

				// can only create an addendum if all reporting steps for the procedure are terminated
				if (!CollectionUtils.TrueForAll(procedure.ReportingProcedureSteps,
					delegate(ReportingProcedureStep ps) { return ps.IsTerminated; }))
					return false;

				return true;
			}

			public override bool CanExecute(ReportingProcedureStep step, Staff executingStaff)
			{
				return CanExecute(step.Procedure, executingStaff);
			}
		}

		public class ReviseUnpublishedReport : ReportingOperation
		{
			public VerificationStep Execute(PublicationStep step, Staff executingStaff, IWorkflow workflow)
			{
				// Discontinue the publication step
				step.Discontinue();

				// Create a new verification step that uses the same report part
				VerificationStep verification = new VerificationStep(step);

				// Reset the verifier
				verification.ReportPart.Verifier = null;

				// Assign the new step back to me
				verification.Assign(executingStaff);
				verification.Start(executingStaff);

				workflow.AddActivity(verification);
				return verification;
			}

			public override bool CanExecute(ReportingProcedureStep step, Staff executingStaff)
			{
				if (!step.Is<PublicationStep>())
					return false;

				// must be a scheduled publication
				if (step.State != ActivityStatus.SC)
					return false;

				// can only revise reports verified by the same staff
				if (!Equals(step.ReportPart.Verifier, executingStaff))
					return false;

				return true;
			}
		}

		/// <summary>
		/// Publishes the report. 
		/// </summary>
		public class PublishReport : ReportingOperation
		{
			public void Execute(PublicationStep step, Staff executingStaff, IWorkflow workflow)
			{
				if (step.AssignedStaff != null)
					step.Complete(executingStaff);
				else
					step.Complete();
			}

			public override bool CanExecute(ReportingProcedureStep step, Staff executingStaff)
			{
				if (!step.Is<PublicationStep>())
					return false;

				// must be a scheduled publication step
				if (step.State != ActivityStatus.SC)
					return false;

				// can only publish reports verified by the same staff
				if (!Equals(step.ReportPart.Verifier, executingStaff))
					return false;

				return true;
			}
		}
	}
}
