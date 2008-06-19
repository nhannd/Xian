using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Client;

namespace ClearCanvas.Ris.Client.Emergency
{
	[ExtensionPoint]
	public class EmergencyHomeFolderSystemToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[MenuAction("launch", "global-menus/Go/Emergency Home", "Launch")]
	//[ButtonAction("launch", "global-toolbars/Go/Radiologist Home", "Launch")]
	[Tooltip("launch", "Emergency Home")]
	// TODO: change icons
	[IconSet("launch", IconScheme.Colour, "Icons.EmergencyPhysicianHomeToolSmall.png", "Icons.EmergencyPhysicianHomeToolMedium.png", "Icons.EmergencyPhysicianHomeToolLarge.png")]
	//[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	public class EmergencyHomeTool : WorklistPreviewHomeTool<EmergencyHomeFolderSystemToolExtensionPoint>
	{
		public override string Title
		{
			get { return SR.TitleEmergencyHome; }
		}
	}
}