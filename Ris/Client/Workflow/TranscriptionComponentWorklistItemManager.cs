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
using ClearCanvas.Ris.Application.Common.TranscriptionWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	public class TranscriptionComponentWorklistItemManager : WorklistItemManager<ReportingWorklistItemSummary, ITranscriptionWorkflowService>
	{
		public TranscriptionComponentWorklistItemManager(string folderName, EntityRef worklistRef, string worklistClassName)
			: base(folderName, worklistRef, worklistClassName, TranscriptionSettings.Default.NextItemQueueCacheSize)
		{
		}

		protected override IContinuousWorkflowComponentMode GetMode(ReportingWorklistItemSummary worklistItem)
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
		public static IContinuousWorkflowComponentMode Create = new CreateTranscriptionComponentMode();
		public static IContinuousWorkflowComponentMode Edit = new EditTranscriptionComponentMode();
		public static IContinuousWorkflowComponentMode Review = new ReviewTranscriptionComponentMode();
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