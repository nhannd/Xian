using System.Drawing;
using ClearCanvas.Dicom.Iod.Sequences;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.PresentationStates;

namespace ClearCanvas.ImageViewer.PresentationStates.GraphicAnnotationSerializers
{
	public class RectangleGraphicAnnotationSerializer : GraphicAnnotationSerializer<IBoundableGraphic>
	{
		protected override void Serialize(IBoundableGraphic graphic, GraphicAnnotationSequenceItem serializationState)
		{
			if (!graphic.Visible)
				return; // if the graphic is not visible, don't serialize it!

			GraphicAnnotationSequenceItem.GraphicObjectSequenceItem annotationElement = new GraphicAnnotationSequenceItem.GraphicObjectSequenceItem();

			graphic.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				annotationElement.GraphicAnnotationUnits = GraphicAnnotationSequenceItem.GraphicAnnotationUnits.Pixel;
				annotationElement.GraphicDimensions = 2;
				annotationElement.GraphicType = GraphicAnnotationSequenceItem.GraphicType.Polyline;
				annotationElement.NumberOfGraphicPoints = 5;

				RectangleF bounds = graphic.BoundingBox;

				// add shape vertices
				PointF[] list = new PointF[5];
				list[0] = bounds.Location;
				list[1] = bounds.Location + new SizeF(bounds.Width, 0);
				list[2] = bounds.Location + bounds.Size;
				list[3] = bounds.Location + new SizeF(0, bounds.Height);
				list[4] = bounds.Location;
				annotationElement.GraphicData = list;

				annotationElement.GraphicFilled = GraphicAnnotationSequenceItem.GraphicFilled.N;
			}
			finally
			{
				graphic.ResetCoordinateSystem();
			}

			serializationState.AppendGraphicObjectSequence(annotationElement);
		}
	}
}