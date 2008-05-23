using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client
{
	[ExtensionPoint]
	public class GlobalHomeFolderSystemToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[MenuAction("launch", "global-menus/Go/Global Home", "Launch")]
	//[ButtonAction("launch", "global-toolbars/Go/Global Home", "Launch")]
	[Tooltip("launch", "Global Home")]
	[IconSet("launch", IconScheme.Colour, "Icons.GlobalHomeToolSmall.png", "Icons.GlobalHomeToolMedium.png", "Icons.GlobalHomeToolLarge.png")]
	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	public class GlobalHomeTool : WorklistPreviewHomeTool<GlobalHomeFolderSystemToolExtensionPoint>
	{
		public override string Title
		{
			get { return "Global Home"; }
		}
	}
}
