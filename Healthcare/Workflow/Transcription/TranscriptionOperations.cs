using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Workflow.Transcription
{
	public class TranscriptionOperations
	{
		public abstract class TranscriptionOperation
		{
			public virtual bool CanExecute(TranscriptionStep step, Staff currentUserStaff)
			{
				return false;
			}
		}

		public class StartTranscription : TranscriptionOperation
		{
			public void Execute(TranscriptionStep step, Staff performingStaff)
			{
				step.Start(performingStaff);
			}

			public override bool CanExecute(TranscriptionStep step, Staff currentUserStaff)
			{
				if (!step.IsInitial
					|| step.State == ActivityStatus.IP && !Equals(step.PerformingStaff, currentUserStaff))
					return false;

				if (step.Scheduling != null && !Equals(step.AssignedStaff, currentUserStaff))
					return false;

				return true;
			}
		}

		public class DiscardTranscription : TranscriptionOperation
		{
			public void Execute(TranscriptionStep step, Staff performingStaff)
			{
				step.Discontinue();
				TranscriptionStep transcriptionStep = new TranscriptionStep(step);
				step.Procedure.AddProcedureStep(transcriptionStep);
				transcriptionStep.Schedule(Platform.Time);
			}

			public override bool CanExecute(TranscriptionStep step, Staff currentUserStaff)
			{
				if (step.State != ActivityStatus.IP)
					return false;

				if (!Equals(step.PerformingStaff, currentUserStaff))
					return false;

				return true;
			}
		}

		public class SaveTranscription : TranscriptionOperation
		{
			public void Execute(TranscriptionStep step, Dictionary<string, string> reportPartExtendedProperties)
			{
				foreach (KeyValuePair<string, string> pair in reportPartExtendedProperties)
				{
					step.ReportPart.ExtendedProperties[pair.Key] = pair.Value;
				}
			}

			public override bool CanExecute(TranscriptionStep step, Staff currentUserStaff)
			{
				if (step.State != ActivityStatus.IP)
					return false;

				if (!Equals(step.PerformingStaff, currentUserStaff))
					return false;

				return true;
			}
		}

		public class SubmitTranscriptionForReview : TranscriptionOperation
		{
			public void Execute(TranscriptionStep step, Staff performingStaff, Staff supervisor)
			{
				step.Complete();

				TranscriptionStep transcriptionStep = new TranscriptionStep(step);
				step.Procedure.AddProcedureStep(transcriptionStep);
				transcriptionStep.Assign(supervisor);
				transcriptionStep.Schedule(Platform.Time);
			}

			public override bool CanExecute(TranscriptionStep step, Staff currentUserStaff)
			{
				if (step.State != ActivityStatus.IP)
					return false;

				if (!Equals(step.PerformingStaff, currentUserStaff))
					return false;

				return true;
			}
		}

		public class CompleteTranscription : TranscriptionOperation
		{
			public void Execute(TranscriptionStep step, Staff performingStaff)
			{
				if(step.State == ActivityStatus.SC)
					step.Complete(performingStaff);
				else
					step.Complete();

				TranscriptionReviewStep transcriptionReviewStep = new TranscriptionReviewStep(step);
				step.Procedure.AddProcedureStep(transcriptionReviewStep);
				transcriptionReviewStep.Assign(step.ReportPart.Interpreter);
				transcriptionReviewStep.Schedule(Platform.Time);
			}

			public override bool CanExecute(TranscriptionStep step, Staff currentUserStaff)
			{
				if (step.State != ActivityStatus.SC && step.State != ActivityStatus.IP)
					return false;

				if (step.State == ActivityStatus.IP && !Equals(step.PerformingStaff, currentUserStaff))
					return false;

				// scheduled items should should look like items submitted for review
				if (step.State == ActivityStatus.SC && (step.ReportPart.Transcriber == null || step.AssignedStaff == null))
					return false;

				// if it looks like an transcription submitted for review, but not to the current user.
				if (step.State == ActivityStatus.SC && step.ReportPart.Transcriber != null && !Equals(step.AssignedStaff, currentUserStaff))
					return false;

				return true;
			}
		}

		public class RejectTranscription : TranscriptionOperation
		{
			public void Execute(TranscriptionStep step, Staff rejectedBy, string reason)
			{
				if (step.State == ActivityStatus.SC)
					step.Complete(rejectedBy);
				else
					step.Complete();

				TranscriptionReviewStep transcriptionReviewStep = new TranscriptionReviewStep(step);
				step.Procedure.AddProcedureStep(transcriptionReviewStep);
				transcriptionReviewStep.Assign(step.ReportPart.Interpreter);
				transcriptionReviewStep.Schedule(Platform.Time);
				transcriptionReviewStep.HasErrors = true;
			}

			public override bool CanExecute(TranscriptionStep step, Staff currentUserStaff)
			{
				if (step.State != ActivityStatus.SC && step.State != ActivityStatus.IP)
					return false;

				if (step.State == ActivityStatus.IP && !Equals(step.PerformingStaff, currentUserStaff))
					return false;

				// scheduled items should should look like items submitted for review
				if (step.State == ActivityStatus.SC && (step.ReportPart.Transcriber == null || step.AssignedStaff == null))
					return false;

				// if it looks like an transcription submitted for review, but not to the current user.
				if (step.State == ActivityStatus.SC && step.ReportPart.Transcriber != null && !Equals(step.AssignedStaff, currentUserStaff))
					return false;

				return true;
			}
		}
	}
}
