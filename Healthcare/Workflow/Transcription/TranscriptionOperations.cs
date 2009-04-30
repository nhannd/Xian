#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

				if (step.AssignedStaff != null && !Equals(step.AssignedStaff, currentUserStaff))
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

			public void Execute(TranscriptionStep step, Dictionary<string, string> reportPartExtendedProperties, Staff supervisor)
			{
				this.Execute(step, reportPartExtendedProperties);

				step.ReportPart.TranscriptionSupervisor = supervisor;
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

				// if it looks like a transcription submitted for review, but not to the current user.
				if (step.State == ActivityStatus.SC && step.ReportPart.Transcriber != null && !Equals(step.AssignedStaff, currentUserStaff))
					return false;

				return true;
			}
		}

		public class RejectTranscription : TranscriptionOperation
		{
			public void Execute(TranscriptionStep step, Staff rejectedBy, TranscriptionRejectReasonEnum reason)
			{
				if (step.State == ActivityStatus.SC)
					step.Complete(rejectedBy);
				else
					step.Complete();

				step.ReportPart.TranscriptionRejectReason = reason;

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
