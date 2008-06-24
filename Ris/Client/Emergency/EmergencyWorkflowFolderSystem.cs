using System;
using System.Security.Permissions;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client;
using ClearCanvas.Ris.Client.Adt;
using ClearCanvas.Ris.Client.Adt.Folders;
using ClearCanvas.Ris.Client.Emergency.Folders;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.Emergency
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
	[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.FolderSystems.Emergency)]
	public class EmergencyWorkflowFolderSystem
		: RegistrationWorkflowFolderSystemBase<EmergencyWorkflowFolderExtensionPoint, EmergencyWorkflowFolderToolExtensionPoint,
			EmergencyWorkflowItemToolExtensionPoint>
	{
		public EmergencyWorkflowFolderSystem()
			: base(SR.TitleEmergencyFolderSystem)
		{
		}

		protected override string GetPreviewUrl()
		{
			return WebResourcesSettings.Default.EmergencyPhysicianFolderSystemUrl;
		}

        protected override SearchResultsFolder CreateSearchResultsFolder()
        {
            return new RegistrationSearchFolder();
        }
    }
}