#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	public class ReportingComponentWorklistItemManager : WorklistItemManager<ReportingWorklistItemSummary, IReportingWorkflowService>
	{
		public ReportingComponentWorklistItemManager(string folderName, EntityRef worklistRef, string worklistClassName)
			: base(folderName, worklistRef, worklistClassName, ReportingSettings.Default.NextItemQueueCacheSize)
		{
		}

		protected override IContinuousWorkflowComponentMode GetMode(ReportingWorklistItemSummary worklistItem)
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
						? ReportingComponentModes.CreateAddendum
						: worklistItem.ReportRef == null ? ReportingComponentModes.Create : ReportingComponentModes.Edit;
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
		public static IContinuousWorkflowComponentMode Create = new CreateReportComponentMode();
		public static IContinuousWorkflowComponentMode CreateAddendum = new CreateReportAddendumComponentMode();
		public static IContinuousWorkflowComponentMode Edit = new EditReportComponentMode();
		public static IContinuousWorkflowComponentMode ReviewTranscription = new ReviewTranscriptionReportComponentMode();
		public static IContinuousWorkflowComponentMode Review = new ReviewReportComponentMode();
		public static IContinuousWorkflowComponentMode Verify = new VerifyReportComponentMode();
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