using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Graphics;
using System.Collections.Generic;
using ClearCanvas.ImageViewer.DicomGraphics;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("showHide", "global-menus/MenuTools/MenuStandard/MenuShowHideDicomOverlay", "ShowHide")]
	[Tooltip("showHide", "TooltipShowHideDicomOverlay")]
	[GroupHint("showHide", "Tools.Image.Overlays.DicomOverlay.ShowHide")]
	[IconSet("showHide", IconScheme.Colour, "Icons.DicomOverlaysToolSmall.png", "Icons.DicomOverlaysToolMedium.png", "Icons.DicomOverlaysToolLarge.png")]
	//
	[ButtonAction("toggle", "overlays-dropdown/ToolbarDicomOverlay", "ShowHide")]
	[CheckedStateObserver("toggle", "Checked", "CheckedChanged")]
	[Tooltip("toggle", "TooltipDicomOverlay")]
	[GroupHint("toggle", "Tools.Image.Overlays.DicomOverlay.ShowHide")]
	[IconSet("toggle", IconScheme.Colour, "Icons.DicomOverlaysToolSmall.png", "Icons.DicomOverlaysToolMedium.png", "Icons.DicomOverlaysToolLarge.png")]
	//
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class DicomOverlaysTool : OverlayToolBase
	{
		public DicomOverlaysTool()
		{
		}

		private static IEnumerable<OverlayPlanesGraphic> GetOverlayPlanesGraphic(IDicomPresentationImage image)
		{
			foreach (OverlayPlanesGraphic overlayGraphic in CollectionUtils.Select(image.DicomGraphics,
				delegate(IGraphic graphic) { return graphic is OverlayPlanesGraphic; }))
			{
				yield return overlayGraphic;
			}
		}

		protected override void  UpdateVisibility(IPresentationImage image, bool visible)
		{
			if (image is IDicomPresentationImage)
			{
				foreach (OverlayPlanesGraphic overlayGraphic in GetOverlayPlanesGraphic(image as IDicomPresentationImage))
					overlayGraphic.Visible = Checked;
			}
		}
	}
}