using ClearCanvas.Common;
using System.Security.Permissions;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	[ExtensionOf(typeof(GlobalHomeFolderSystemToolExtensionPoint))]
	[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.FolderSystems.OrderNotes)]
	public class OrderNoteboxFolderSystemTool : FolderExplorerToolBase
	{
		public override void Initialize()
		{
			base.Initialize();

			_folderSystem = new OrderNoteboxFolderSystem(this.Context);
		}
	}
}