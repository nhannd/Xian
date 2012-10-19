#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	[ExtensionPoint]
	public class ReportingWorkflowFolderExtensionPoint : ExtensionPoint<IWorklistFolder>
	{
	}

	[ExtensionPoint]
	public class ReportingWorkflowItemToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionPoint]
	public class ReportingWorkflowFolderToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionOf(typeof(FolderSystemExtensionPoint))]
	[PrincipalPermission(SecurityAction.Demand, Role = Application.Common.AuthorityTokens.FolderSystems.Reporting)]
	public class ReportingWorkflowFolderSystem
		: ReportingWorkflowFolderSystemBase<ReportingWorkflowFolderExtensionPoint, ReportingWorkflowFolderToolExtensionPoint,
			ReportingWorkflowItemToolExtensionPoint>
	{
		public ReportingWorkflowFolderSystem()
			: base(SR.TitleReportingFolderSystem)
		{
		}

		protected override void AddDefaultFolders()
		{
			// add the personal folders, since they are not extensions and will not be automatically added
			this.Folders.Add(new Folders.Reporting.AssignedFolder());

			var workflowConfig = new WorkflowConfigurationReader();
			if (workflowConfig.EnableInterpretationReviewWorkflow && CurrentStaffCanSupervise())
			{
				this.Folders.Add(new Folders.Reporting.AssignedForReviewFolder());
			}

			this.Folders.Add(new Folders.Reporting.DraftFolder());

			if (workflowConfig.EnableTranscriptionWorkflow)
			{
				this.Folders.Add(new Folders.Reporting.InTranscriptionFolder());
				this.Folders.Add(new Folders.Reporting.ReviewTranscriptionFolder());
			}

			if (workflowConfig.EnableInterpretationReviewWorkflow && 
				Thread.CurrentPrincipal.IsInRole(Application.Common.AuthorityTokens.Workflow.Report.SubmitForReview))
				this.Folders.Add(new Folders.Reporting.AwaitingReviewFolder());

			this.Folders.Add(new Folders.Reporting.VerifiedFolder());
		}

		protected override string GetPreviewUrl(WorkflowFolder folder, ICollection<ReportingWorklistItemSummary> items)
		{
			return WebResourcesSettings.Default.ReportingFolderSystemUrl;
		}

		protected override PreviewOperationAuditData[] GetPreviewAuditData(WorkflowFolder folder, ICollection<ReportingWorklistItemSummary> items)
		{
			return items.Select(item => new PreviewOperationAuditData("Reporting", item)).ToArray();
		}

		protected override SearchResultsFolder CreateSearchResultsFolder()
		{
			return new Folders.Reporting.ReportingSearchFolder();
		}
	}
}
