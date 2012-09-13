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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tools;
using System.Threading;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	[ExtensionPoint]
	public class RegistrationWorkflowFolderExtensionPoint : ExtensionPoint<IWorklistFolder>
	{
	}

	[ExtensionPoint]
	public class RegistrationWorkflowItemToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionPoint]
	public class RegistrationWorkflowFolderToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionOf(typeof(FolderSystemExtensionPoint))]
	[PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.FolderSystems.Registration)]
	public class RegistrationWorkflowFolderSystem
		: RegistrationWorkflowFolderSystemBase<RegistrationWorkflowFolderExtensionPoint, RegistrationWorkflowFolderToolExtensionPoint,
			RegistrationWorkflowItemToolExtensionPoint>
	{
		public RegistrationWorkflowFolderSystem()
			: base(SR.TitleRegistrationFolderSystem)
		{
		}

		protected override string GetPreviewUrl(WorkflowFolder folder, ICollection<RegistrationWorklistItemSummary> items)
		{
			return WebResourcesSettings.Default.RegistrationFolderSystemUrl;
		}

		protected override PreviewOperationAuditData[] GetPreviewAuditData(WorkflowFolder folder, ICollection<RegistrationWorklistItemSummary> items)
		{
			return items.Select(item => new PreviewOperationAuditData("Registration", item)).ToArray();
		}

        protected override SearchResultsFolder CreateSearchResultsFolder()
        {
            return new Folders.Registration.RegistrationSearchFolder();
        }
    }
}