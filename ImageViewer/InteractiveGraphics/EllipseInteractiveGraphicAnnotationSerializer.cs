using System.Drawing;
using ClearCanvas.Dicom.Iod.Sequences;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.PresentationStates;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	internal class EllipseInteractiveGraphicAnnotationSerializer : GraphicAnnotationSerializer<EllipseInteractiveGraphic>
	{
		protected override void Serialize(EllipseInteractiveGraphic graphic, GraphicAnnotationSequenceItem serializationState)
		{
			if (!graphic.Visible)
				return; // if the graphic is not visible, don't serialize it!

			GraphicAnnotationSequenceItem.GraphicObjectSequenceItem annotationElement = new GraphicAnnotationSequenceItem.GraphicObjectSequenceItem();

			graphic.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				annotationElement.GraphicAnnotationUnits = GraphicAnnotationSequenceItem.GraphicAnnotationUnits.Pixel;
				annotationElement.GraphicDimensions = 2;
				annotationElement.GraphicFilled = GraphicAnnotationSequenceItem.GraphicFilled.N;

				SizeF halfDims = new SizeF(graphic.Width/2, graphic.Height/2);
				if (FloatComparer.AreEqual(graphic.Width, graphic.Height)) // check if graphic is a circle
				{
					annotationElement.GraphicType = GraphicAnnotationSequenceItem.GraphicType.Circle;
					annotationElement.NumberOfGraphicPoints = 2;

					PointF[] list = new PointF[2];
					list[0] = graphic.TopLeft + halfDims; // centre of circle
					list[1] = graphic.TopLeft + new SizeF(0, halfDims.Height); // any point on the circle
					annotationElement.GraphicData = list;
				}
				else
				{
					annotationElement.GraphicType = GraphicAnnotationSequenceItem.GraphicType.Ellipse;
					annotationElement.NumberOfGraphicPoints = 4;

					int offset = graphic.Width < graphic.Height ? 2 : 0; // offset list by 2 if major axis is vertical
					PointF[] list = new PointF[4];
					list[(offset + 0)%4] = graphic.TopLeft + new SizeF(0, halfDims.Height); // left point of horizontal axis
					list[(offset + 1)%4] = graphic.TopLeft + new SizeF(graphic.Width, halfDims.Height); // right point of horizontal axis
					list[(offset + 2)%4] = graphic.TopLeft + new SizeF(halfDims.Width, 0); // top point of vertical axis
					list[(offset + 3)%4] = graphic.TopLeft + new SizeF(halfDims.Width, graphic.Height); // bottom point of vertical axis
					annotationElement.GraphicData = list;
				}
			}
			finally
			{
				graphic.ResetCoordinateSystem();
			}

			serializationState.AppendGraphicObjectSequence(annotationElement);
		}
	}
}