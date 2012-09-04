#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common;
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
		private readonly WorkflowConfigurationReader _workflowConfiguration;

		public TranscriptionWorkflowFolderSystem()
			: base(SR.TitleTranscriptionFolderSystem)
		{
			_workflowConfiguration = new WorkflowConfigurationReader();
			if(!_workflowConfiguration.EnableTranscriptionWorkflow)
				throw new NotSupportedException("Transcription workflow has been disabled in the system configuration.");
		}

		protected override void AddDefaultFolders()
		{

			if (_workflowConfiguration.EnableTranscriptionReviewWorkflow)
			{
				this.Folders.Add(new Folders.Transcription.ToBeReviewedFolder());
			}

			this.Folders.Add(new Folders.Transcription.DraftFolder());

			if (_workflowConfiguration.EnableTranscriptionReviewWorkflow &&
				Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Transcription.SubmitForReview))
				this.Folders.Add(new Folders.Transcription.AwaitingReviewFolder());

			this.Folders.Add(new Folders.Transcription.CompletedFolder());
		}

		protected override string GetPreviewUrl(WorkflowFolder folder, ICollection<ReportingWorklistItemSummary> items)
		{
			return WebResourcesSettings.Default.TranscriptionFolderSystemUrl;
		}

		protected override SearchResultsFolder CreateSearchResultsFolder()
		{
			return new Folders.Transcription.TranscriptionSearchFolder();
		}
	}
}