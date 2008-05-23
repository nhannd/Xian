using ClearCanvas.Common;

namespace ClearCanvas.Ris.Client
{
	[ExtensionOf(typeof(GlobalHomeFolderSystemToolExtensionPoint))]
	public class OrderNoteboxFolderSystemTool : FolderExplorerToolBase
	{
		public override void Initialize()
		{
			base.Initialize();

			_folderSystem = new OrderNoteboxFolderSystem(this.Context);
		}
	}
}