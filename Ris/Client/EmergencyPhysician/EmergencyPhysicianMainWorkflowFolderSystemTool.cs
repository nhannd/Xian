using System.Security.Permissions;
using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client;
using ClearCanvas.Ris.Client.EmergencyPhysician;

namespace ClearCanvas.Ris.Client.EmergencyPhysician
{
	[ExtensionOf(typeof(GlobalHomeFolderSystemToolExtensionPoint))]
	[ExtensionOf(typeof(EmergencyPhysicianHomeFolderSystemToolExtensionPoint))]
	[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.FolderSystems.Emergency)]
	public class EmergencyPhysicianMainWorkflowFolderSystemTool : FolderExplorerToolBase
	{
		public override void Initialize()
		{
			base.Initialize();

			_folderSystem = new EmergencyWorkflowFolderSystem(this.Context);
		}
	}
}