using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Application.Common.TranscriptionWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	public class TranscriptionComponentWorklistItemManager : WorklistItemManager<ReportingWorklistItem, ITranscriptionWorkflowService>
	{
		public TranscriptionComponentWorklistItemManager(string folderName, EntityRef worklistRef, string worklistClassName)
			: base(folderName, worklistRef, worklistClassName)
		{
		}

		protected override IContinuousWorkflowComponentMode GetMode<TWorklistITem>(ReportingWorklistItem worklistItem)
		{
			if (worklistItem == null)
				return TranscriptionComponentModes.Review;

			switch (worklistItem.ActivityStatus.Code)
			{
				case StepState.Scheduled:
					return TranscriptionComponentModes.Create;
				case StepState.InProgress:
					return TranscriptionComponentModes.Edit;
				default:
					return TranscriptionComponentModes.Review;
			}
		}

		protected override string TaskName
		{
			get { return "Transcribing"; }
		}
	}

	public class TranscriptionComponentModes
	{
		public static CreateTranscriptionComponentMode Create = new CreateTranscriptionComponentMode();
		public static EditTranscriptionComponentMode Edit = new EditTranscriptionComponentMode();
		public static ReviewTranscriptionComponentMode Review = new ReviewTranscriptionComponentMode();
	}

	public class EditTranscriptionComponentMode : ContinuousWorkflowComponentMode
	{
		public EditTranscriptionComponentMode() : base(false, false, false)
		{
		}
	}

	public class CreateTranscriptionComponentMode : ContinuousWorkflowComponentMode
	{
		public CreateTranscriptionComponentMode() : base(true, true, true)
		{
		}
	}

	public class ReviewTranscriptionComponentMode : ContinuousWorkflowComponentMode
	{
		public ReviewTranscriptionComponentMode() : base(false, false, false)
		{
		}
	}
}