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

using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	public class ReportingComponentWorklistItemManager : WorklistItemManager<ReportingWorklistItem, IReportingWorkflowService>
	{
		public ReportingComponentWorklistItemManager(string folderName, EntityRef worklistRef, string worklistClassName)
			: base(folderName, worklistRef, worklistClassName)
		{
		}

		protected override IContinuousWorkflowComponentMode GetMode<TWorklistITem>(ReportingWorklistItem worklistItem)
		{
			if (worklistItem == null)
				return ReportingComponentModes.Review;

			if (worklistItem.ProcedureStepName == StepType.Publication)
				return ReportingComponentModes.Review;

			if (worklistItem.ProcedureStepName == StepType.Verification)
				return ReportingComponentModes.Verify;

			if (worklistItem.ProcedureStepName == StepType.TranscriptionReview)
				return ReportingComponentModes.ReviewTranscription;

			switch (worklistItem.ActivityStatus.Code)
			{
				case StepState.Scheduled:
					return worklistItem.IsAddendumStep 
						? (IContinuousWorkflowComponentMode) ReportingComponentModes.CreateAddendum
						: ReportingComponentModes.Create;
				case StepState.InProgress:
					return ReportingComponentModes.Edit;
				default:
					return ReportingComponentModes.Review;
			}
		}

		protected override string TaskName
		{
			get { return "Reporting"; }
		}
	}

	public class ReportingComponentModes
	{
		public static CreateReportComponentMode Create = new CreateReportComponentMode();
		public static CreateReportAddendumComponentMode CreateAddendum = new CreateReportAddendumComponentMode();
		public static EditReportComponentMode Edit = new EditReportComponentMode();
		public static ReviewTranscriptionReportComponentMode ReviewTranscription = new ReviewTranscriptionReportComponentMode();
		public static ReviewReportComponentMode Review = new ReviewReportComponentMode();
		public static VerifyReportComponentMode Verify = new VerifyReportComponentMode();
	}

	public class EditReportComponentMode : ContinuousWorkflowComponentMode
	{
		public EditReportComponentMode()
			: base(false, false, false)
		{
		}
	}

	public class CreateReportComponentMode : ContinuousWorkflowComponentMode
	{
		public CreateReportComponentMode()
			: base(true, true, true)
		{
		}
	}

	public class CreateReportAddendumComponentMode : ContinuousWorkflowComponentMode
	{
		public CreateReportAddendumComponentMode()
			: base(true, false, false)
		{
		}
	}

	public class ReviewReportComponentMode : ContinuousWorkflowComponentMode
	{
		public ReviewReportComponentMode()
			: base(false, false, false)
		{
		}
	}

	public class VerifyReportComponentMode : ContinuousWorkflowComponentMode
	{
		public VerifyReportComponentMode()
			: base(false, false, true)
		{
		}
	}

	public class ReviewTranscriptionReportComponentMode : ContinuousWorkflowComponentMode
	{
		public ReviewTranscriptionReportComponentMode()
			: base(false, false, true)
		{
		}
	}
}