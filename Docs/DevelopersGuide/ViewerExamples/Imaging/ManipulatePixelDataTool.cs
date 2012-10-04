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
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;

// ... (other using namespace statements here)

namespace MyPlugin.Imaging
{
	[ButtonAction("apply", "global-toolbars/ToolbarStandard/ManipulatePixelDataTool", "Apply")]
	[ButtonAction("apply", "global-toolbars/ToolbarStandard/ResetManipulatePixelDataTool", "Unapply")]
	[IconSet("apply", IconScheme.Colour, "Icons.MyToolSmall.png", "Icons.MyToolMedium.png", "Icons.MyToolLarge.png")]
	// ... (other action attributes here)
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class ManipulatePixelDataTool : ImageViewerTool
	{
		public void Apply()
		{
			// Check for nulls before we using anything
			if (this.SelectedImageGraphicProvider == null)
				return;

			ImageGraphic image = this.SelectedImageGraphicProvider.ImageGraphic;

			if (image == null)
				return;

			// Make sure the image is grayscale
			if (!(image is GrayscaleImageGraphic))
				return;

			// Change the grayscale value of the pixel at (4,7) (8th row, 5th column)
			image.PixelData.SetPixel(4, 7, 100);

			// Redraw the onscreen image
			image.Draw();

			// We've modified the original pixel data!
			// To reload it, we will need to call the other method.
		}

		public void Unapply()
		{
			// Check for nulls before we using anything
			if (this.SelectedImageSopProvider == null)
				return;

			this.SelectedImageSopProvider.Frame.UnloadPixelData();

			// Redraw the onscreen image
			this.SelectedPresentationImage.Draw();
		}
	}
}