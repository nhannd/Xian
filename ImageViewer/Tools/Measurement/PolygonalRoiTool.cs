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
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	[MenuAction("activate", "imageviewer-contextmenu/MenuPolygonalRoi", "Select", Flags = ClickActionFlags.CheckAction, InitiallyAvailable = false)]
	[MenuAction("activate", "global-menus/MenuTools/MenuMeasurement/MenuPolygonalRoi", "Select", Flags = ClickActionFlags.CheckAction)]
	[ButtonAction("activate", "global-toolbars/ToolbarMeasurement/ToolbarPolygonalRoi", "Select", Flags = ClickActionFlags.CheckAction)]
	[CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]
	[MouseButtonIconSet("activate", IconScheme.Colour, "Icons.PolygonalRoiToolSmall.png", "Icons.PolygonalRoiToolMedium.png", "Icons.PolygonalRoiToolLarge.png")]
    [GroupHint("activate", "Tools.Image.Annotations.Measurement.Roi.Polygonal")]
	[MouseToolButton(XMouseButtons.Left, false)]
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class PolygonalRoiTool : MeasurementTool
	{
		public PolygonalRoiTool() : base(SR.TooltipPolygonalRoi) {}

		protected override string CreationCommandName
		{
			get { return SR.CommandCreatePolygonalRoi; }
		}

		protected override string RoiNameFormat
		{
			get { return SR.FormatPolygonName; }
		}

		protected override IGraphic CreateGraphic()
		{
			return new PolygonControlGraphic(true, new MoveControlGraphic(new PolylineGraphic(true)));
		}

		protected override InteractiveGraphicBuilder CreateGraphicBuilder(IGraphic graphic)
		{
			return new InteractivePolygonGraphicBuilder((IPointsGraphic) graphic);
		}

		protected override IAnnotationCalloutLocationStrategy CreateCalloutLocationStrategy()
		{
			return new PolygonalRoiCalloutLocationStrategy();
		}
	}
}