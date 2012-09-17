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
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	[ExtensionPoint]
	public class PerformingWorkflowFolderExtensionPoint : ExtensionPoint<IWorklistFolder>
	{
	}

	[ExtensionPoint]
	public class PerformingWorkflowItemToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionPoint]
	public class PerformingWorkflowFolderToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionOf(typeof(FolderSystemExtensionPoint))]
	[PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.FolderSystems.Performing)]
	public class PerformingWorkflowFolderSystem
		: PerformingWorkflowFolderSystemBase<PerformingWorkflowFolderExtensionPoint, PerformingWorkflowFolderToolExtensionPoint,
			PerformingWorkflowItemToolExtensionPoint>
	{
		public PerformingWorkflowFolderSystem()
			: base(SR.TitlePerformingFolderSystem)
		{
		}

		protected override string GetPreviewUrl(WorkflowFolder folder, ICollection<ModalityWorklistItemSummary> items)
		{
			return WebResourcesSettings.Default.PerformingFolderSystemUrl;
		}

		protected override PreviewOperationAuditData[] GetPreviewAuditData(WorkflowFolder folder, ICollection<ModalityWorklistItemSummary> items)
		{
			return items.Select(item => new PreviewOperationAuditData("Performing", item)).ToArray();
		}

		protected override SearchResultsFolder CreateSearchResultsFolder()
        {
            return new Folders.Performing.SearchFolder();
        }
	}
}
