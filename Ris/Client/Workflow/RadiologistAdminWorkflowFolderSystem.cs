using System.Collections.Generic;
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

		protected override string GetPreviewUrl(WorkflowFolder folder, ICollection<ReportingWorklistItem> items)
		{
			if (items.Count == 0)
				return null;

			string stepName = CollectionUtils.FirstElement(items).ProcedureStepName;

			bool hasDifferentStepName = CollectionUtils.Contains(items,
				delegate(ReportingWorklistItem item) { return !item.ProcedureStepName.Equals(stepName); });

			// Don't show a preview if the items have different type of steps
			if (hasDifferentStepName)
				return null;

			return GetPreviewFromStepName(stepName);
		}

        protected override SearchResultsFolder CreateSearchResultsFolder()
        {
			return new Folders.RadiologistAdmin.RadiologistAdminSearchFolder();
        }

		private static string GetPreviewFromStepName(string stepName)
		{
			//TODO: having the client specify the step name name may not be a terribly good idea
			switch(stepName)
			{
				case "Interpretation":
				case "Transcription":
				case "Verification":
				case "Publication":
					return WebResourcesSettings.Default.ReportingFolderSystemUrl;
				case "ProtocolAssignment":
				case "ProtocolResolution":
					return WebResourcesSettings.Default.ProtocollingFolderSystemUrl;
				default:
					return null;
			}
		}
    }
}
