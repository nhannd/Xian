using ClearCanvas.Common;
using ClearCanvas.Ris.Client;
using ClearCanvas.Ris.Client.EmergencyPhysician;

namespace ClearCanvas.Ris.Client.EmergencyPhysician
{
	[ExtensionOf(typeof(EmergencyPhysicianHomeFolderSystemToolExtensionPoint))]
	public class EmergencyPhysicianMainWorkflowFolderSystemTool : FolderExplorerToolBase
	{
		public override void Initialize()
		{
			base.Initialize();

			_folderSystem = new EmergencyPhysicianMainWorkflowFolderSystem(this.Context);
		}
	}
}