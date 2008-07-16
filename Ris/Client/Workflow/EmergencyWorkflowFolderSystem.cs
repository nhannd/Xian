using System.Collections.Generic;
using System.Security.Permissions;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Client;

namespace ClearCanvas.Ris.Client.Workflow
{
	[ExtensionPoint]
	public class EmergencyWorkflowFolderExtensionPoint : ExtensionPoint<IWorklistFolder>
	{
	}

	[ExtensionPoint]
	public class EmergencyWorkflowItemToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionPoint]
	public class EmergencyWorkflowFolderToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionOf(typeof(FolderSystemExtensionPoint))]
	[PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.FolderSystems.Emergency)]
	public class EmergencyWorkflowFolderSystem
		: RegistrationWorkflowFolderSystemBase<EmergencyWorkflowFolderExtensionPoint, EmergencyWorkflowFolderToolExtensionPoint,
			EmergencyWorkflowItemToolExtensionPoint>
	{
		public EmergencyWorkflowFolderSystem()
			: base(SR.TitleEmergencyFolderSystem)
		{
		}

		protected override string GetPreviewUrl(WorkflowFolder folder, ICollection<RegistrationWorklistItem> items)
		{
			return WebResourcesSettings.Default.EmergencyPhysicianFolderSystemUrl;
		}

        protected override SearchResultsFolder CreateSearchResultsFolder()
        {
            return new Folders.Registration.RegistrationSearchFolder();
        }
    }
}