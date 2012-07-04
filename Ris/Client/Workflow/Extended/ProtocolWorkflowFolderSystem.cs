#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Security.Permissions;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
	[ExtensionPoint]
	public class ProtocolWorkflowFolderExtensionPoint : ExtensionPoint<IWorklistFolder>
	{
	}

	[ExtensionPoint]
	public class ProtocolWorkflowItemToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionPoint]
	public class ProtocolWorkflowFolderToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionOf(typeof(FolderSystemExtensionPoint))]
	[PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.FolderSystems.Protocolling)]
	public class ProtocolWorkflowFolderSystem
		: ReportingWorkflowFolderSystemBase<ProtocolWorkflowFolderExtensionPoint, ProtocolWorkflowFolderToolExtensionPoint,
			ProtocolWorkflowItemToolExtensionPoint>
	{
		public ProtocolWorkflowFolderSystem()
			: base(SR.TitleProtocollingFolderSystem)
		{
		}

		protected override void AddDefaultFolders()
		{
			// add the personal folders, since they are not extensions and will not be automatically added
			this.Folders.Add(new Folders.Reporting.AssignedToBeProtocolFolder());

			if (CurrentStaffCanSupervise())
			{
				this.Folders.Add(new Folders.Reporting.AssignedForReviewProtocolFolder());
			}
			this.Folders.Add(new Folders.Reporting.DraftProtocolFolder());

			if (Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Protocol.SubmitForReview))
				this.Folders.Add(new Folders.Reporting.AwaitingApprovalProtocolFolder());

			this.Folders.Add(new Folders.Reporting.CompletedProtocolFolder());
			this.Folders.Add(new Folders.Reporting.RejectedProtocolFolder());
		}

		protected override string GetPreviewUrl(WorkflowFolder folder, ICollection<ReportingWorklistItemSummary> items)
		{
			return WebResourcesSettings.Default.ProtocollingFolderSystemUrl;
		}

		protected override SearchResultsFolder CreateSearchResultsFolder()
		{
			return new Folders.Reporting.ProtocollingSearchFolder();
		}
	}
}
