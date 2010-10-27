#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.Filter
{
	[MenuAction("apply", "global-menus/MenuTools/MenuFilter/MenuReset", "Apply")]
	[MenuAction("apply", "imageviewer-filterdropdownmenu/MenuReset", "Apply")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class ResetFilterTool : ImageViewerTool
	{
		public ResetFilterTool() {}

		public void Apply()
		{
			if (this.SelectedImageGraphicProvider == null)
				return;

			ImageGraphic image = this.SelectedImageGraphicProvider.ImageGraphic;
			if (image == null)
				return;

			if (!(image is GrayscaleImageGraphic))
				return;

			// explicitly unload the (filtered) pixel data for the frame
			this.SelectedImageSopProvider.Frame.UnloadPixelData();

			// now draw the frame again - since we unloaded the filtered pixel data, it will reload from the original source
			image.Draw();
		}
	}
}