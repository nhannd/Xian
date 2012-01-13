using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.Automation
{
    public interface IDrawEllipse
    {
        AnnotationGraphic Draw(CoordinateSystem coordinateSystem, string name, PointF topLeft, PointF bottomRight);
    }

    public interface IDrawPolygon
    {
        AnnotationGraphic Draw(CoordinateSystem coordinateSystem, string name, IList<PointF> vertices);
    }

    public interface IDrawProtractor
    {
        AnnotationGraphic Draw(CoordinateSystem coordinateSystem, string name, PointF point1, PointF vertex, PointF point2);
    }

    public interface IDrawRectangle
    {
        AnnotationGraphic Draw(CoordinateSystem coordinateSystem, string name, PointF topLeft, PointF bottomRight);
    }

    public interface IDrawRuler
    {
        AnnotationGraphic Draw(CoordinateSystem coordinateSystem, string name, PointF point1, PointF point2);
    }

    public interface IDrawTextArea
    {
        IControlGraphic Draw(CoordinateSystem coordinateSystem, string name, string text, PointF textLocation);
    }

    public interface IDrawTextCallout
    {
        IControlGraphic Draw(CoordinateSystem coordinateSystem, string name, PointF anchorPoint, string text, PointF textLocation);
    }
}