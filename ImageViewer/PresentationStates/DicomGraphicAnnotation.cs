using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Dicom.Iod.Sequences;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.PresentationStates
{
	[DicomSerializableGraphicAnnotation(typeof (DicomGraphicAnnotationSerializer))]
	public class DicomGraphicAnnotation : CompositeGraphic
	{
		private Color _color = Color.Yellow;

		public DicomGraphicAnnotation(GraphicAnnotationSequenceItem serializationState, RectangleF displayedArea)
		{
			this.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				List<PointF> dataPoints = new List<PointF>();
				if (serializationState.GraphicObjectSequence != null)
				{
					foreach (GraphicAnnotationSequenceItem.GraphicObjectSequenceItem graphicItem in serializationState.GraphicObjectSequence)
					{
						IList<PointF> points = GetGraphicDataAsSourceCoordinates(displayedArea, graphicItem);
						switch (graphicItem.GraphicType)
						{
							case GraphicAnnotationSequenceItem.GraphicType.Interpolated:
								base.Graphics.Add(CreateInterpolated(points));
								break;
							case GraphicAnnotationSequenceItem.GraphicType.Polyline:
								base.Graphics.Add(CreatePolyline(points));
								break;
							case GraphicAnnotationSequenceItem.GraphicType.Point:
								base.Graphics.Add(CreatePoint(points[0]));
								break;
							case GraphicAnnotationSequenceItem.GraphicType.Circle:
								base.Graphics.Add(CreateCircle(points[0], (float) Vector.Distance(points[0], points[1])));
								break;
							case GraphicAnnotationSequenceItem.GraphicType.Ellipse:
								base.Graphics.Add(CreateEllipse(points[0], points[1], points[2], points[3]));
								break;
							default:
								break;
						}
						dataPoints.AddRange(points);
					}
				}

				RectangleF annotationBounds = RectangleUtilities.ComputeBoundingRectangle(dataPoints.ToArray());
				if (serializationState.TextObjectSequence != null)
				{
					foreach (GraphicAnnotationSequenceItem.TextObjectSequenceItem textItem in serializationState.TextObjectSequence)
					{
						CalloutGraphic callout = CreateCalloutText(annotationBounds, textItem);
						base.Graphics.Add(callout);
					}
				}
			}
			finally
			{
				this.ResetCoordinateSystem();
			}
		}

		public sealed override CoordinateSystem CoordinateSystem {
			get {
				return base.CoordinateSystem;
			}
			set {
				base.CoordinateSystem = value;
			}
		}

		public sealed override void ResetCoordinateSystem() {
			base.ResetCoordinateSystem();
		}

		public Color Color
		{
			get { return _color; }
			set
			{
				if (_color != value)
				{
					_color = value;
					OnColorChanged();
				}
			}
		}

		private void OnColorChanged()
		{
			foreach (IGraphic graphic in base.Graphics)
			{
				if (graphic is PolyLineGraphic)
					((PolyLineGraphic) graphic).Color = _color;
				else if (graphic is VectorGraphic)
					((VectorGraphic) graphic).Color = _color;
			}
		}

		private static IList<PointF> GetGraphicDataAsSourceCoordinates(RectangleF displayedArea, GraphicAnnotationSequenceItem.GraphicObjectSequenceItem graphicItem)
		{
			List<PointF> list;
			if (graphicItem.GraphicAnnotationUnits == GraphicAnnotationSequenceItem.GraphicAnnotationUnits.Display)
			{
				list = new List<PointF>(graphicItem.NumberOfGraphicPoints);
				foreach (PointF point in graphicItem.GraphicData)
					list.Add(displayedArea.Location + new SizeF(displayedArea.Width*point.X, displayedArea.Height*point.Y));
			}
			else
			{
				list = new List<PointF>(graphicItem.GraphicData);
			}
			return list.AsReadOnly();
		}

		private static IGraphic CreateInterpolated(IList<PointF> dataPoints)
		{
			// linear interpolation!
			// we will eventually implement cubic spline interpolation
			return CreatePolyline(dataPoints);
		}

		private static IGraphic CreatePolyline(IList<PointF> vertices)
		{
			PolyLineGraphic polyline = new PolyLineGraphic();
			for (int n = 0; n < vertices.Count; n++)
				polyline.Add(vertices[n]);
			return polyline;
		}

		private static IGraphic CreateCircle(PointF center, float radius)
		{
			EllipsePrimitive circle = new EllipsePrimitive();
			SizeF radial = new SizeF(radius, radius);
			circle.TopLeft = center - radial;
			circle.BottomRight = center + radial;
			return circle;
		}

		private static IGraphic CreatePoint(PointF location)
		{
			const float radius = 5;

			InvariantEllipsePrimitive point = new InvariantEllipsePrimitive();
			point.AnchorPoint = location;
			point.InvariantTopLeft = new PointF(-radius, -radius);
			point.InvariantBottomRight = new PointF(radius, radius);
			return point;
		}

		private static IGraphic CreateEllipse(PointF majorAxisEnd1, PointF majorAxisEnd2, PointF minorAxisEnd1, PointF minorAxisEnd2)
		{
			EllipsePrimitive ellipse = new EllipsePrimitive();
			PointF centroid = Vector.Midpoint(majorAxisEnd1, majorAxisEnd2);
			SizeF radial = new SizeF((float) (Vector.Distance(majorAxisEnd1, majorAxisEnd2)/2), (float) (Vector.Distance(minorAxisEnd1, minorAxisEnd2)/2));
			ellipse.TopLeft = centroid - radial;
			ellipse.BottomRight = centroid + radial;
			ellipse.SpatialTransform.CenterOfRotationXY = centroid;
			ellipse.SpatialTransform.RotationXY = (int) Vector.SubtendedAngle(majorAxisEnd1, majorAxisEnd2, majorAxisEnd2 + new SizeF(1, 0))%180;
			return ellipse;
		}

		private static CalloutGraphic CreateCalloutText(RectangleF annotationBounds, GraphicAnnotationSequenceItem.TextObjectSequenceItem textItem) {
			CalloutGraphic callout = new CalloutGraphic();
			callout.Text = textItem.UnformattedTextValue;

			if (textItem.AnchorPoint.HasValue)
			{
				callout.EndPoint = textItem.AnchorPoint.Value;

				callout.CoordinateSystem = CoordinateSystem.Destination;
				callout.Location = annotationBounds.Location - new SizeF(30, 30);
				callout.ResetCoordinateSystem();
			}
			return callout;
		}

		private class DicomGraphicAnnotationSerializer : GraphicAnnotationSerializer<DicomGraphicAnnotation>
		{
			protected override void Serialize(DicomGraphicAnnotation graphic, GraphicAnnotationSequenceItem serializationState) {}
		}
	}
}