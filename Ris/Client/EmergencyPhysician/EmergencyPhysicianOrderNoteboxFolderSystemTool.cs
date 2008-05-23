using ClearCanvas.Common;
using ClearCanvas.Ris.Client.EmergencyPhysician;

namespace ClearCanvas.Ris.Client.EmergencyPhysician
{
	[ExtensionOf(typeof(GlobalHomeFolderSystemToolExtensionPoint))]
	[ExtensionOf(typeof(EmergencyPhysicianHomeFolderSystemToolExtensionPoint))]
	public class EmergencyPhysicianOrderNoteboxFolderSystemTool : FolderExplorerToolBase
	{
		public override void Initialize()
		{
			base.Initialize();

			_folderSystem = new EmergencyPhysicianOrderNoteboxFolderSystem(this.Context);
		}
	}
}