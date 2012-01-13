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
using ClearCanvas.ImageViewer.Automation;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	[MenuAction("activate", "imageviewer-contextmenu/MenuRuler", "Select", Flags = ClickActionFlags.CheckAction)]
	[MenuAction("activate", "global-menus/MenuTools/MenuMeasurement/MenuRuler", "Select", Flags = ClickActionFlags.CheckAction)]
	[ButtonAction("activate", "global-toolbars/ToolbarMeasurement/ToolbarRuler", "Select", Flags = ClickActionFlags.CheckAction)]
	[CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]
	[MouseButtonIconSet("activate", "Icons.RulerToolSmall.png", "Icons.RulerToolMedium.png", "Icons.RulerToolLarge.png")]
    [GroupHint("activate", "Tools.Image.Annotations.Measurement.Roi.Linear")]

	[MouseToolButton(XMouseButtons.Left, false)]
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public partial class RulerTool : MeasurementTool
	{
		public RulerTool()
			: base(SR.TooltipRuler)
		{
		}

		protected override string CreationCommandName
		{
			get { return SR.CommandCreateRuler; }
		}

		protected override string RoiNameFormat
		{
			get { return SR.FormatRulerName; }
		}

		protected override IGraphic CreateGraphic()
		{
			return new VerticesControlGraphic(new MoveControlGraphic(new PolylineGraphic()));
		}

		protected override InteractiveGraphicBuilder CreateGraphicBuilder(IGraphic graphic)
		{
			return new InteractivePolylineGraphicBuilder(2, (IPointsGraphic) graphic);
		}

		protected override IAnnotationCalloutLocationStrategy CreateCalloutLocationStrategy()
		{
			return new RulerCalloutLocationStrategy();
		}
    }

    #region Oto
    partial class RulerTool : IDrawRuler
    {
        AnnotationGraphic IDrawRuler.Draw(CoordinateSystem coordinateSystem, string name, PointF point1, PointF point2)
        {
            var image = Context.Viewer.SelectedPresentationImage;
            if (!CanStart(image))
                throw new InvalidOperationException("Can't draw an elliptical ROI at this time.");

            var imageGraphic = ((IImageGraphicProvider) image).ImageGraphic;
            if (coordinateSystem == CoordinateSystem.Destination)
            {
                //Use the image graphic to get the "source" coordinates because it's already in the scene.
                point1 = imageGraphic.SpatialTransform.ConvertToSource(point1);
                point2 = imageGraphic.SpatialTransform.ConvertToSource(point2);
            }

            var overlayProvider = (IOverlayGraphicsProvider) image;
            var roiGraphic = CreateRoiGraphic(false);
            roiGraphic.Name = name;
            AddRoiGraphic(image, roiGraphic, overlayProvider);

            var subject = (IPointsGraphic)roiGraphic.Subject;

            subject.Points.Add(point1);
            subject.Points.Add(point2);

            roiGraphic.Callout.Update();
            roiGraphic.State = roiGraphic.CreateSelectedState();
            //roiGraphic.Draw();
            return roiGraphic;
        }
    }
    #endregion
}