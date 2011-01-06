#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("showHide", "imageviewer-contextmenu/MenuShowHideTextOverlay", "ShowHide", InitiallyAvailable = false)]
	[MenuAction("showHide", "global-menus/MenuTools/MenuStandard/MenuShowHideTextOverlay", "ShowHide")]
	[Tooltip("showHide", "TooltipShowHideTextOverlay")]
	[GroupHint("showHide", "Tools.Image.Overlays.Text.ShowHide")]
	[IconSet("showHide", IconScheme.Colour, "Icons.TextOverlayToolSmall.png", "Icons.TextOverlayToolMedium.png", "Icons.TextOverlayToolLarge.png")]
	//
	[ButtonAction("toggle", "overlays-dropdown/ToolbarTextOverlay", "ShowHide")]
	[CheckedStateObserver("toggle", "Checked", "CheckedChanged")]
	[Tooltip("toggle", "TooltipTextOverlay")]
	[GroupHint("toggle", "Tools.Image.Overlays.Text.ShowHide")]
	[IconSet("toggle", IconScheme.Colour, "Icons.TextOverlayToolSmall.png", "Icons.TextOverlayToolMedium.png", "Icons.TextOverlayToolLarge.png")]
	//
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class TextOverlayTool : OverlayToolBase
	{
		public TextOverlayTool()
		{
		}

		protected override void  UpdateVisibility(IPresentationImage image, bool visible)
		{
			if (image is IAnnotationLayoutProvider)
			{
				foreach (AnnotationBox box in ((IAnnotationLayoutProvider)image).AnnotationLayout.AnnotationBoxes)
					box.Visible = visible;
			}
		}
	}
}
