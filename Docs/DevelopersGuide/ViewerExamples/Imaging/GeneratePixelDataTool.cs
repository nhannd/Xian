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

// ... (other using namespace statements here)

namespace MyPlugin.Imaging
{
	[ButtonAction("apply", "global-toolbars/ToolbarStandard/GeneratePixelDataTool", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.MyToolSmall.png", "Icons.MyToolMedium.png", "Icons.MyToolLarge.png")]
	// ... (other action attributes here)
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class GeneratePixelDataTool : ImageViewerTool
	{
		public void Apply()
		{
			// create some pixel data
			ColorImageGraphic colorImage = new ColorImageGraphic(256, 256);

			colorImage.PixelData.ForEachPixel(
				delegate(int i, int x, int y, int pixelIndex)
					{
						if (y > 128)
							colorImage.PixelData.SetPixel(pixelIndex, Color.Blue);
						else
							colorImage.PixelData.SetPixel(pixelIndex, Color.Red);
					});

			// now do something with it... like add it to the image
			IOverlayGraphicsProvider provider = base.SelectedPresentationImage as IOverlayGraphicsProvider;
			if (provider == null)
				return;

			// reposition it relative to the image
			colorImage.SpatialTransform.TranslationX = 5;
			colorImage.SpatialTransform.TranslationY = 5;

			provider.OverlayGraphics.Add(colorImage);
			base.SelectedPresentationImage.Draw();
		}
	}
}