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
		[FolderPath("To be Transcribed", true)]
		public class ToBeTranscribedFolder : TranscriptionWorkflowFolder
		{
		}

		[FolderForWorklistClass(WorklistClassNames.TranscriptionDraftWorklist)]
		[FolderPath("My Items/Draft")]
		public class DraftFolder : TranscriptionWorkflowFolder
		{
		}

		[FolderForWorklistClass(WorklistClassNames.TranscriptionToBeReviewedWorklist)]
		[FolderPath("My Items/To Be Reviewed")]
		public class ToBeReviewedFolder : TranscriptionWorkflowFolder
		{
		}

		[FolderForWorklistClass(WorklistClassNames.TranscriptionAwaitingReviewWorklist)]
		[FolderPath("My Items/Awaiting Review")]
		public class AwaitingReviewFolder : TranscriptionWorkflowFolder
		{
		}

		[FolderForWorklistClass(WorklistClassNames.TranscriptionCompletedWorklist)]
		[FolderPath("My Items/Completed")]
		public class CompletedFolder : TranscriptionWorkflowFolder
		{
		}

		[FolderPath("Search Results")]
		public class TranscriptionSearchFolder : WorklistSearchResultsFolder<ReportingWorklistItem, ITranscriptionWorkflowService>
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