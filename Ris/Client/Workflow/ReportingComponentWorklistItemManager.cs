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