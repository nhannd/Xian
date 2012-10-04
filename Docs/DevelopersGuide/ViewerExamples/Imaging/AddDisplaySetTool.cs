#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.BaseTools;

// ... (other using namespace statements here)

namespace MyPlugin.Imaging
{
	[ButtonAction("apply", "global-toolbars/ToolbarStandard/AddDisplaySetTool", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.MyToolSmall.png", "Icons.MyToolMedium.png", "Icons.MyToolLarge.png")]
	// ... (other action attributes here)
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class AddDisplaySetTool : ImageViewerTool
	{
		public void Apply()
		{
			if (this.ImageViewer == null)
				return;

			if (this.ImageViewer.SelectedImageBox == null)
				return;

			// Create volume from currently selected DisplaySet

			// in this case, we'll need to use the VolumePresentationImage
			// in the ClearCanvas.ImageViewer.Tools.Volume.VTK project
			IDisplaySet selectedDisplaySet = this.ImageViewer.SelectedImageBox.DisplaySet;
			VolumePresentationImage image = new VolumePresentationImage(selectedDisplaySet);

			// Create new DisplaySet   
			IDisplaySet displaySet = new DisplaySet(
				String.Format("{0} (3D)", selectedDisplaySet.Name),
				DicomUid.GenerateUid().ToString());

			// Add the VolumePresentationImage
			displaySet.PresentationImages.Add(image);
			this.ImageViewer.LogicalWorkspace.ImageSets[0].DisplaySets.Add(displaySet);

			// Put DisplaySet in ImageBox and render
			IImageBox imageBox = this.ImageViewer.SelectedImageBox;
			imageBox.DisplaySet = displaySet;
			imageBox.Draw();
		}
	}
}