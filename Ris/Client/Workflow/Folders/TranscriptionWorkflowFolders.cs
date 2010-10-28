#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Application.Common.TranscriptionWorkflow;

namespace ClearCanvas.Ris.Client.Workflow.Folders
{
	public class Transcription
	{
		[ExtensionOf(typeof(TranscriptionWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.TranscriptionToBeTranscribedWorklist)]
		[FolderPath("To Be Transcribed", true)]
		[FolderDescription("TranscriptionToBeTranscribedFolderDescription")]
		public class ToBeTranscribedFolder : TranscriptionWorkflowFolder
		{
		}

		[FolderForWorklistClass(WorklistClassNames.TranscriptionDraftWorklist)]
		[FolderPath("My Items/Draft")]
		[FolderDescription("TranscriptionDraftFolderDescription")]
		public class DraftFolder : TranscriptionWorkflowFolder
		{
		}

		[FolderForWorklistClass(WorklistClassNames.TranscriptionToBeReviewedWorklist)]
		[FolderPath("My Items/To Be Reviewed")]
		[FolderDescription("TranscriptionToBeReviewedFolderDescription")]
		public class ToBeReviewedFolder : TranscriptionWorkflowFolder
		{
		}

		[FolderForWorklistClass(WorklistClassNames.TranscriptionAwaitingReviewWorklist)]
		[FolderPath("My Items/Awaiting Review")]
		[FolderDescription("TranscriptionAwaitingReviewFolderDescription")]
		public class AwaitingReviewFolder : TranscriptionWorkflowFolder
		{
		}

		[FolderForWorklistClass(WorklistClassNames.TranscriptionCompletedWorklist)]
		[FolderPath("My Items/Completed")]
		[FolderDescription("TranscriptionCompletedFolderDescription")]
		public class CompletedFolder : TranscriptionWorkflowFolder
		{
		}

		[FolderPath("Search Results")]
		public class TranscriptionSearchFolder : WorklistSearchResultsFolder<ReportingWorklistItemSummary, ITranscriptionWorkflowService>
		{
			public TranscriptionSearchFolder()
				: base(new ReportingWorklistTable())
			{
			}

			//TODO: (JR may 2008) having the client specify the class name isn't a terribly good idea, but
			//it is the only way to get things working right now
			protected override string ProcedureStepClassName
			{
				get { return "ReportingProcedureStep"; }
			}

		}
	}
}