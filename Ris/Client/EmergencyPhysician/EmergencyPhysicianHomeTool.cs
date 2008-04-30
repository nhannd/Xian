using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Client;

namespace ClearCanvas.Ris.Client.EmergencyPhysician
{
	[ExtensionPoint]
	public class EmergencyPhysicianHomeFolderSystemToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[MenuAction("launch", "global-menus/Go/Emergency Physician Home", "Launch")]
	//[ButtonAction("launch", "global-toolbars/Go/Radiologist Home", "Launch")]
	[Tooltip("launch", "Emergency Physician Home")]
	// TODO: change icons
	[IconSet("launch", IconScheme.Colour, "Icons.EmergencyPhysicianHomeToolSmall.png", "Icons.EmergencyPhysicianHomeToolMedium.png", "Icons.EmergencyPhysicianHomeToolLarge.png")]
	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	public class EmergencyPhysicianHomeTool : WorklistPreviewHomeTool<EmergencyPhysicianHomeFolderSystemToolExtensionPoint>
	{
		public override string Title
		{
			get { return SR.TitleEmergencyPhysicianHome; }
		}
	}
}