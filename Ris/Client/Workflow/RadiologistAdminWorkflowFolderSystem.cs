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
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	[ExtensionPoint]
	public class RadiologistAdminWorkflowFolderExtensionPoint : ExtensionPoint<IWorklistFolder>
	{
	}

	[ExtensionPoint]
	public class RadiologistAdminWorkflowItemToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionPoint]
	public class RadiologistAdminWorkflowFolderToolExtensionPoint : ExtensionPoint<ITool>
	{
	}


	/// <summary>
	/// A folder system that allowing radiologist admin to monitor personal items and reassign to another radiologist
	/// </summary>
	[ExtensionOf(typeof(FolderSystemExtensionPoint))]
	[PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.FolderSystems.RadiologistAdmin)]
	public class RadiologistAdminWorkflowFolderSystem
		: ReportingWorkflowFolderSystemBase<RadiologistAdminWorkflowFolderExtensionPoint, RadiologistAdminWorkflowFolderToolExtensionPoint,
			RadiologistAdminWorkflowItemToolExtensionPoint>
	{
		public RadiologistAdminWorkflowFolderSystem()
			: base(SR.TitleRadiologistAdminFolderSystem)
		{
		}

		protected override string GetPreviewUrl(WorkflowFolder folder, ICollection<ReportingWorklistItemSummary> items)
		{
			if (items.Count != 1)
				return null;

			var reportingWorklistItem = CollectionUtils.FirstElement(items);

			//TODO: having the client specify the step name name may not be a terribly good idea
			switch (reportingWorklistItem.ProcedureStepName)
			{
				case "Interpretation":
				case "Transcription":
				case "Transcription Review":
				case "Verification":
				case "Publication":
					return WebResourcesSettings.Default.ReportingFolderSystemUrl;
				case "Protocol Assignment":
				case "Protocol Resolution":
					return WebResourcesSettings.Default.ProtocollingFolderSystemUrl;
				default:
					return null;
			}
		}

		protected override PreviewOperationAuditData[] GetPreviewAuditData(WorkflowFolder folder, ICollection<ReportingWorklistItemSummary> items)
		{
			return items.Select(item => new PreviewOperationAuditData("Radiologist Admin", item)).ToArray();
		}

		public override bool AdvancedSearchEnabled
		{
			get { return false; }
		}

		public override string SearchMessage
		{
			get { return SR.MessageRadiologistAdminSearchMessage; }
		}

		protected override SearchResultsFolder CreateSearchResultsFolder()
		{
			return new Folders.RadiologistAdmin.RadiologistAdminSearchFolder();
		}
	}
}
