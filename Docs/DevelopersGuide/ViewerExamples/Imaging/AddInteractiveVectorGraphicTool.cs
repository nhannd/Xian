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

// ... (other using namespace statements here)

namespace MyPlugin.Imaging
{
	[ButtonAction("apply", "global-toolbars/ToolbarStandard/AddInteractiveVectorGraphicTool", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.MyToolSmall.png", "Icons.MyToolMedium.png", "Icons.MyToolLarge.png")]
	// ... (other action attributes here)
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class AddInteractiveVectorGraphicTool : ImageViewerTool
	{
		public void Apply()
		{
			if (this.SelectedOverlayGraphicsProvider == null)
				return;

			// Create a composite graphic
			CompositeGraphic composite = new CompositeGraphic();

			// Create a rectangle
			RectanglePrimitive rect = new RectanglePrimitive();
			rect.TopLeft = new PointF(5, 10);
			rect.BottomRight = new PointF(20, 25);

			// Create a line
			LinePrimitive line = new LinePrimitive();
			line.Point1 = new PointF(5, 25);
			line.Point2 = new PointF(20, 10);

			// Add the rectangle to a move control graphic so that the user can move the entire rectangle around
			// and a resize control graphic so that the user can resize the rectangle
			MoveControlGraphic rectangleMoveControlGraphic = new MoveControlGraphic(rect);
			BoundableResizeControlGraphic rectangleResizeControlGraphic = new BoundableResizeControlGraphic(rectangleMoveControlGraphic);

			// Add the line to a move control graphic so that the user can move the entire rectangle around
			// and a line control graphic so that the user can change the endpoints of the line
			MoveControlGraphic lineMoveControlGraphic = new MoveControlGraphic(line);
			LineSegmentStretchControlGraphic lineStretchControlGraphic = new LineSegmentStretchControlGraphic(lineMoveControlGraphic);

			// Add the rectangle and line to the composite
			composite.Graphics.Add(rectangleResizeControlGraphic);
			composite.Graphics.Add(lineStretchControlGraphic);

			// Add the composite to the selected presentation image's scene graph      
			this.SelectedOverlayGraphicsProvider.OverlayGraphics.Add(composite);

			// Now render
			this.SelectedPresentationImage.Draw();
		}
	}
}