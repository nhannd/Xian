using System.Security.Permissions;
using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client;
using ClearCanvas.Ris.Client.Emergency;

namespace ClearCanvas.Ris.Client.Emergency
{
	[ExtensionOf(typeof(GlobalHomeFolderSystemToolExtensionPoint))]
	[ExtensionOf(typeof(EmergencyHomeFolderSystemToolExtensionPoint))]
	[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.FolderSystems.Emergency)]
	public class EmergencyWorkflowFolderSystemTool : FolderExplorerToolBase
	{
		public override void Initialize()
		{
			base.Initialize();

			_folderSystem = new EmergencyWorkflowFolderSystem(this.Context);
		}
	}
}