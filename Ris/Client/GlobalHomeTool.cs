using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
	[ExtensionPoint]
	public class FolderSystemExtensionPoint : ExtensionPoint<IFolderSystem>
	{
	}

	[MenuAction("launch", "global-menus/Go/Home", "Launch")]
	[Tooltip("launch", "Go to home page")]
	[IconSet("launch", IconScheme.Colour, "Icons.GlobalHomeToolSmall.png", "Icons.GlobalHomeToolMedium.png", "Icons.GlobalHomeToolLarge.png")]
	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	public class GlobalHomeTool : WorklistPreviewHomeTool<FolderSystemExtensionPoint>
	{
		public override void Initialize()
		{
			base.Initialize();

			// automatically launch home page on startup
			Launch();
		}

		public override string Title
		{
			get { return "Home"; }
		}
	}
}
