using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.DicomGraphics;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("showHide", "global-menus/MenuTools/MenuStandard/MenuShowHideShutterOverlay", "ShowHide")]
	[Tooltip("showHide", "TooltipShowHideShutterOverlay")]
	[GroupHint("showHide", "Tools.Image.Overlays.Shutter.ShowHide")]
	[IconSet("showHide", IconScheme.Colour, "Icons.ShutterOverlayToolSmall.png", "Icons.ShutterOverlayToolMedium.png", "Icons.ShutterOverlayToolLarge.png")]
	//
	[ButtonAction("toggle", "overlays-dropdown/ToolbarShutterOverlay", "ShowHide")]
	[CheckedStateObserver("toggle", "Checked", "CheckedChanged")]
	[Tooltip("toggle", "TooltipShutterOverlay")]
	[GroupHint("toggle", "Tools.Image.Overlays.Shutter.ShowHide")]
	[IconSet("toggle", IconScheme.Colour, "Icons.ShutterOverlayToolSmall.png", "Icons.ShutterOverlayToolMedium.png", "Icons.ShutterOverlayToolLarge.png")]
	//
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class ShutterOverlayTool : OverlayToolBase
	{
		public ShutterOverlayTool()
		{
		}

		private static IEnumerable<ShuttersGraphic> GetShuttersGraphics(IDicomPresentationImage image)
		{
			foreach (ShuttersGraphic shuttersGraphic in CollectionUtils.Select(image.DicomGraphics,
				delegate(IGraphic graphic) { return graphic is ShuttersGraphic; }))
			{
				yield return shuttersGraphic;
			}
		}

		protected override void UpdateVisibility(IPresentationImage image, bool visible)
		{
			if (image is IDicomPresentationImage)
			{
				foreach (ShuttersGraphic shuttersGraphic in GetShuttersGraphics(image as IDicomPresentationImage))
					shuttersGraphic.Visible = Checked;
			}
		}
	}
}