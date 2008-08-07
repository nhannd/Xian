using System.Collections.Generic;
using System.Security.Permissions;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Common.Utilities;

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

		protected override string GetPreviewUrl(WorkflowFolder folder, ICollection<ReportingWorklistItem> items)
		{
			if (items.Count == 0)
				return null;

			string procedureStepName = CollectionUtils.FirstElement(items).ProcedureStepName;

			bool hasDifferentProcedureStep = CollectionUtils.Contains(items,
				delegate(ReportingWorklistItem item) { return !item.ProcedureStepName.Equals(procedureStepName); });

			if (hasDifferentProcedureStep)
				return null;

				return WebResourcesSettings.Default.ProtocollingFolderSystemUrl;
			//else
			//    return WebResourcesSettings.Default.ReportingFolderSystemUrl;
		}

        protected override SearchResultsFolder CreateSearchResultsFolder()
        {
        	return new Folders.Reporting.ReportingSearchFolder();
        }
    }
}
