#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.PresentationStates;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;

// ... (other using namespace statements here)

namespace MyPlugin.DicomImaging
{
	[ButtonAction("apply", "global-toolbars/ToolbarStandard/AddGeometricShutterTool", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.MyToolSmall.png", "Icons.MyToolMedium.png", "Icons.MyToolLarge.png")]
	// ... (other action attributes here)
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class AddGeometricShutterTool : ImageViewerTool
	{
		public void Apply()
		{
			IDicomPresentationImage image = this.SelectedPresentationImage as IDicomPresentationImage;

			if (image == null)
				return;

			// Get the base image
			ImageGraphic baseImage = image.ImageGraphic;

			// Get the DICOM graphics plane
			DicomGraphicsPlane dicomGraphicsPlane = DicomGraphicsPlane.GetDicomGraphicsPlane(image, true);

			// Create the largest possible circular shutter graphic that is still smaller than the image itself
			GeometricShuttersGraphic shutter = new GeometricShuttersGraphic(baseImage.Rows, baseImage.Columns);
			shutter.CustomShutters.Add(new CircularShutter(new Point(baseImage.Columns/2, baseImage.Rows/2), Math.Min(baseImage.Rows, baseImage.Columns)/2));

			// Add the shutter to the collection of available shutters on the image
			dicomGraphicsPlane.Shutters.Add(shutter);

			// Activate the shutter
			dicomGraphicsPlane.Shutters.Activate(shutter);

			// Now render
			image.Draw();
		}
	}
}