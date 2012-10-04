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
	[ButtonAction("apply", "global-toolbars/ToolbarStandard/ApplyMySpatialTransformTool", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.MyToolSmall.png", "Icons.MyToolMedium.png", "Icons.MyToolLarge.png")]
	// ... (other action attributes here)
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class SpatialTransformTool : ImageViewerTool
	{
		public void Apply()
		{
			if (this.SelectedPresentationImage == null)
				return;

			ISpatialTransformProvider provider = this.SelectedPresentationImage as ISpatialTransformProvider;

			if (provider == null)
				return;

			ISpatialTransform transform = provider.SpatialTransform;

			// Do something with the transform object, such as...
			transform.FlipX = true;

			// update the image onscreen
			this.SelectedPresentationImage.Draw();
		}
	}
}