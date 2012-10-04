#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;

// ... (other using namespace statements here)

namespace MyPlugin.DicomImaging
{
	[ButtonAction("apply", "global-toolbars/ToolbarStandard/AddBitmapOverlayTool", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.MyToolSmall.png", "Icons.MyToolMedium.png", "Icons.MyToolLarge.png")]
	// ... (other action attributes here)
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class AddBitmapOverlayTool : ImageViewerTool
	{
		private readonly int _threshold = 1200; // Chosen to perform best on CT images.

		public void Apply()
		{
			IDicomPresentationImage image = this.SelectedPresentationImage as IDicomPresentationImage;

			if (image == null)
				return;

			// Get the base image
			ImageGraphic baseImage = image.ImageGraphic;

			// Get the DICOM graphics plane
			DicomGraphicsPlane dicomGraphicsPlane = DicomGraphicsPlane.GetDicomGraphicsPlane(image, true);

			// Create a custom DICOM overlay plane graphic
			UserOverlayPlaneGraphic overlayPlane = new UserOverlayPlaneGraphic(baseImage.Rows, baseImage.Columns);
			overlayPlane.Label = "OverThreshold";
			overlayPlane.Description = string.Format("Pixels over {0}", _threshold);
			overlayPlane.Origin = new PointF(0, 0);
			overlayPlane.Color = Color.PeachPuff;

			// Iterate through each pixel of the base image and if the pixel value
			// is above a certain threshold, flag the pixel in the overlay accordingly
			baseImage.PixelData.ForEachPixel(
				delegate(int i, int x, int y, int pixelIndex)
					{
						if (baseImage.PixelData.GetPixel(pixelIndex) > _threshold)
							overlayPlane[x, y] = true;
					}
				);

			// Add the overlay to the collection of available overlays on the image
			dicomGraphicsPlane.UserOverlays.Add(overlayPlane);

			// Activate the overlay on the "LAYER1" layer
			dicomGraphicsPlane.UserOverlays.ActivateAsLayer(overlayPlane, "LAYER1");

			// Now render
			image.Draw();
		}
	}
}