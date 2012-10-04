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
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.RoiGraphics;

// ... (other using namespace statements here)

namespace MyPlugin.Imaging
{
	[ButtonAction("apply", "global-toolbars/ToolbarStandard/AddMeasurementTool", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.MyToolSmall.png", "Icons.MyToolMedium.png", "Icons.MyToolLarge.png")]
	// ... (other action attributes here)
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class AddMeasurementTool : ImageViewerTool
	{
		public void Apply()
		{
			if (this.SelectedOverlayGraphicsProvider == null)
				return;

			// Create a rectangle
			RectanglePrimitive rect = new RectanglePrimitive();
			rect.TopLeft = new PointF(5, 10);
			rect.BottomRight = new PointF(20, 25);

			// Add the rectangle to a move control graphic so that the user can move the entire rectangle around
			// and a resize control graphic so that the user can resize the rectangle
			MoveControlGraphic rectangleMoveControlGraphic = new MoveControlGraphic(rect);
			BoundableResizeControlGraphic rectangleResizeControlGraphic = new BoundableResizeControlGraphic(rectangleMoveControlGraphic);

			// Add the controlled rectangle to a ROI graphic
			RoiGraphic roiGraphic = new RoiGraphic(rectangleResizeControlGraphic);

			// Add the rectangular ROI to the selected presentation image's scene graph      
			this.SelectedOverlayGraphicsProvider.OverlayGraphics.Add(roiGraphic);

			// Now render
			this.SelectedPresentationImage.Draw();
		}
	}
}