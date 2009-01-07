using System.Collections.Generic;
using System.Security.Permissions;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	[ExtensionPoint]
	public class TranscriptionWorkflowFolderExtensionPoint : ExtensionPoint<IWorklistFolder>
	{
	}

	[ExtensionPoint]
	public class TranscriptionWorkflowItemToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionPoint]
	public class TranscriptionWorkflowFolderToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionOf(typeof(FolderSystemExtensionPoint))]
	[PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.FolderSystems.Transcription)]
	public class TranscriptionWorkflowFolderSystem
		: ReportingWorkflowFolderSystemBase<TranscriptionWorkflowFolderExtensionPoint, TranscriptionWorkflowFolderToolExtensionPoint,
			TranscriptionWorkflowItemToolExtensionPoint>
	{
		public TranscriptionWorkflowFolderSystem()
			: base(SR.TitleTranscriptionFolderSystem)
		{
			this.Folders.Add(new Folders.Transcription.ToBeReviewedFolder());
			this.Folders.Add(new Folders.Transcription.DraftFolder());

			if (Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Transcription.SubmitForReview))
				this.Folders.Add(new Folders.Transcription.AwaitingReviewFolder());

			this.Folders.Add(new Folders.Transcription.CompletedFolder());
		}

		protected override string GetPreviewUrl(WorkflowFolder folder, ICollection<ReportingWorklistItem> items)
		{
			return WebResourcesSettings.Default.TranscriptionFolderSystemUrl;
		}

		protected override SearchResultsFolder CreateSearchResultsFolder()
		{
			return new Folders.Transcription.TranscriptionSearchFolder();
		}
	}
}