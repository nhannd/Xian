using ClearCanvas.Common;

namespace ClearCanvas.Ris.Client.Reporting
{
	[ExtensionOf(typeof(GlobalHomeFolderSystemToolExtensionPoint))]
	[ExtensionOf(typeof(ReportingHomeFolderSystemToolExtensionPoint))]
	public class ReportingOrderNoteboxFolderSystemTool : FolderExplorerToolBase
	{
		public override void Initialize()
		{
			base.Initialize();

			_folderSystem = new ReportingOrderNoteboxFolderSystem(this.Context);
		}
	}
}
