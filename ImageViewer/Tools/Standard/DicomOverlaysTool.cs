#region License

// Copyright (c) 2011, ClearCanvas Inc.
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
	[MenuAction("showHide", "imageviewer-contextmenu/MenuShowHideDicomOverlay", "ShowHide", InitiallyAvailable = false)]
	[MenuAction("showHide", "global-menus/MenuTools/MenuStandard/MenuShowHideDicomOverlay", "ShowHide")]
	[Tooltip("showHide", "TooltipShowHideDicomOverlay")]
	[GroupHint("showHide", "Tools.Image.Overlays.DicomOverlay.ShowHide")]
	[IconSet("showHide", "Icons.DicomOverlaysToolSmall.png", "Icons.DicomOverlaysToolMedium.png", "Icons.DicomOverlaysToolLarge.png")]
	//
	[ButtonAction("toggle", "overlays-dropdown/ToolbarDicomOverlay", "ShowHide")]
	[CheckedStateObserver("toggle", "Checked", "CheckedChanged")]
	[Tooltip("toggle", "TooltipDicomOverlay")]
	[GroupHint("toggle", "Tools.Image.Overlays.DicomOverlay.ShowHide")]
	[IconSet("toggle", "Icons.DicomOverlaysToolSmall.png", "Icons.DicomOverlaysToolMedium.png", "Icons.DicomOverlaysToolLarge.png")]
	//
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class DicomOverlaysTool : OverlayToolBase
	{
		public DicomOverlaysTool()
		{
		}

		private static IEnumerable<OverlayPlaneGraphic> GetOverlayPlanesGraphic(IDicomPresentationImage image)
		{
			DicomGraphicsPlane dicomGraphicsPlane = DicomGraphicsPlane.GetDicomGraphicsPlane(image, false);
			if (dicomGraphicsPlane != null)
			{
				foreach (ILayer layer in (IEnumerable<ILayer>)dicomGraphicsPlane.Layers)
				{
					foreach (OverlayPlaneGraphic overlayGraphic in CollectionUtils.Select(layer.Graphics,
						delegate(IGraphic graphic) { return graphic is OverlayPlaneGraphic; }))
					{
						yield return overlayGraphic;
					}
				}
			}
		}

		protected override void  UpdateVisibility(IPresentationImage image, bool visible)
		{
			if (image is IDicomPresentationImage)
			{
				foreach (OverlayPlaneGraphic overlayGraphic in GetOverlayPlanesGraphic(image as IDicomPresentationImage))
					overlayGraphic.Visible = Checked;
			}
		}
	}
}