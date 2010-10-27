#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;

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

		protected override void UpdateVisibility(IPresentationImage image, bool visible)
		{
			if (image is IDicomPresentationImage)
			{
				DicomGraphicsPlane dicomGraphics = DicomGraphicsPlane.GetDicomGraphicsPlane(image as IDicomPresentationImage, false);
				if (dicomGraphics != null)
					dicomGraphics.Shutters.Enabled = Checked;
			}
		}
	}
}