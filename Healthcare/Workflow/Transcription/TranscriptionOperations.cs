using System;
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

		public class CompleteTranscription : TranscriptionOperation
		{
			public void Execute(TranscriptionStep step, Staff performingStaff)
			{
				step.Complete();
				VerificationStep verificationStep = new VerificationStep(step);
				step.Procedure.AddProcedureStep(verificationStep);
				verificationStep.Assign(step.ReportPart.Interpreter);
				verificationStep.Schedule(Platform.Time);
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

		public class RejectTranscription : TranscriptionOperation
		{
			public void Execute(TranscriptionStep step, Staff rejectedBy, string reason)
			{
				step.Complete();
				VerificationStep verificationStep = new VerificationStep(step);
				step.Procedure.AddProcedureStep(verificationStep);
				verificationStep.Assign(step.ReportPart.Interpreter);
				verificationStep.Schedule(Platform.Time);
				// TODO: schedule verification step and indicate errors
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
	}
}
