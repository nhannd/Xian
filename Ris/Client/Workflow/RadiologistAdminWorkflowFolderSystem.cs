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
			if (items.Count != 1)
				return null;

			ReportingWorklistItem step = CollectionUtils.FirstElement(items);

			//TODO: having the client specify the step name name may not be a terribly good idea
			switch (step.ProcedureStepName)
			{
				case "Interpretation":
				case "Transcription":
				case "Verification":
				case "Publication":
					return WebResourcesSettings.Default.ReportingFolderSystemUrl;
				case "Protocol":
					return WebResourcesSettings.Default.ProtocollingFolderSystemUrl;
				default:
					return null;
			}
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
