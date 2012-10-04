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
	[ButtonAction("apply", "global-toolbars/ToolbarStandard/AddVectorGraphicTool", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.MyToolSmall.png", "Icons.MyToolMedium.png", "Icons.MyToolLarge.png")]
	// ... (other action attributes here)
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class AddVectorGraphicTool : ImageViewerTool
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

			// Add the rectangle and line to the composite
			composite.Graphics.Add(rect);
			composite.Graphics.Add(line);

			// Add the composite to the selected presentation image's scene graph      
			this.SelectedOverlayGraphicsProvider.OverlayGraphics.Add(composite);

			// Now render
			this.SelectedPresentationImage.Draw();
		}
	}
}