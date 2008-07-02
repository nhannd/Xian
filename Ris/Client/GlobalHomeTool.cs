using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Ris.Client
{
	[ExtensionPoint]
	public class FolderSystemExtensionPoint : ExtensionPoint<IFolderSystem>
	{
	}

	[MenuAction("launch", "global-menus/Go/Home", "Launch")]
	[Tooltip("launch", "Go to home page")]
	[IconSet("launch", IconScheme.Colour, "Icons.GlobalHomeToolSmall.png", "Icons.GlobalHomeToolMedium.png", "Icons.GlobalHomeToolLarge.png")]
	[VisibleStateObserver("launch", "Visible", "VisibleChanged")]
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

		public bool Visible
		{
			get { return this.HasFolderSystems; }
		}

		public event EventHandler VisibleChanged
		{
			add { }
			remove { }
		}
	}
}
