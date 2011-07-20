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
	[MenuAction("activate", "imageviewer-contextmenu/MenuEllipticalRoi", "Select", Flags = ClickActionFlags.CheckAction, InitiallyAvailable = false)]
	[MenuAction("activate", "global-menus/MenuTools/MenuMeasurement/MenuEllipticalRoi", "Select", Flags = ClickActionFlags.CheckAction)]
	[ButtonAction("activate", "global-toolbars/ToolbarMeasurement/ToolbarEllipticalRoi", "Select", Flags = ClickActionFlags.CheckAction)]
    [CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]
	[MouseButtonIconSet("activate", IconScheme.Colour, "Icons.EllipticalRoiToolSmall.png", "Icons.EllipticalRoiToolMedium.png", "Icons.EllipticalRoiToolLarge.png")]
    [GroupHint("activate", "Tools.Image.Annotations.Measurement.Roi.Elliptical")]

	[MouseToolButton(XMouseButtons.Left, false)]
    [ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class EllipticalRoiTool : MeasurementTool
	{
		public EllipticalRoiTool()
			: base(SR.TooltipEllipticalRoi)
		{
		}

		protected override string CreationCommandName
		{
			get { return SR.CommandCreateEllipiticalRoi; }
		}

		protected override string RoiNameFormat
		{
			get { return SR.FormatEllipseName; }
		}

		protected override InteractiveGraphicBuilder CreateGraphicBuilder(IGraphic graphic)
		{
			return new InteractiveBoundableGraphicBuilder((IBoundableGraphic) graphic);
		}

		protected override IAnnotationCalloutLocationStrategy CreateCalloutLocationStrategy()
		{
			return new DefaultRoiCalloutLocationStrategy();
		}

		protected override IGraphic CreateGraphic()
		{
			return new BoundableResizeControlGraphic(new BoundableStretchControlGraphic(new MoveControlGraphic(new EllipsePrimitive())));
		}
	}
}