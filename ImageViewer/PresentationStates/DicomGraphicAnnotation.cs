using System;
using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod.Sequences;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.PresentationStates
{
	/// <summary>
	/// A <see cref="IGraphic"/> whose contents represent those of a DICOM Graphic Annotation Sequence.
	/// </summary>
	[Cloneable]
	[DicomSerializableGraphicAnnotation(typeof (DicomGraphicAnnotationSerializer))]
	public class DicomGraphicAnnotation : CompositeGraphic
	{
		private Color _color = Color.Yellow;

		/// <summary>
		/// Constructs a new <see cref="IGraphic"/> whose contents are constructed based on a <see cref="GraphicAnnotationSequenceItem">DICOM Graphic Annotation Sequence Item</see>.
		/// </summary>
		/// <param name="graphicAnnotationSequenceItem">The DICOM graphic annotation sequence item to render.</param>
		/// <param name="displayedArea">The image's displayed area with which to </param>
		public DicomGraphicAnnotation(GraphicAnnotationSequenceItem graphicAnnotationSequenceItem, RectangleF displayedArea)
		{
			this.CoordinateSystem = CoordinateSystem.Source;

			try
			{
				List<PointF> dataPoints = new List<PointF>();
				if (graphicAnnotationSequenceItem.GraphicObjectSequence != null)
				{
					foreach (GraphicAnnotationSequenceItem.GraphicObjectSequenceItem graphicItem in graphicAnnotationSequenceItem.GraphicObjectSequence)
					{
						try
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
						catch (Exception ex)
						{
							Platform.Log(LogLevel.Warn, ex, "DICOM Softcopy Presentation State Deserialization Fault (Graphic Object Type {0}). Reprocess with log level DEBUG to see DICOM data dump.", graphicItem.GraphicType);
							Platform.Log(LogLevel.Debug, graphicItem.DicomSequenceItem.Dump());
						}
					}
				}

				RectangleF annotationBounds = RectangleUtilities.ComputeBoundingRectangle(dataPoints.ToArray());
				if (graphicAnnotationSequenceItem.TextObjectSequence != null)
				{
					foreach (GraphicAnnotationSequenceItem.TextObjectSequenceItem textItem in graphicAnnotationSequenceItem.TextObjectSequence)
					{
						try
						{
							base.Graphics.Add(CreateCalloutText(annotationBounds, displayedArea, textItem));
						}
						catch (Exception ex)
						{
							Platform.Log(LogLevel.Warn, ex, "DICOM Softcopy Presentation State Deserialization Fault (Text Object). Reprocess with log level DEBUG to see DICOM data dump.");
							Platform.Log(LogLevel.Debug, textItem.DicomSequenceItem.Dump());
						}
					}
				}
			}
			finally
			{
				this.ResetCoordinateSystem();
			}
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected DicomGraphicAnnotation(DicomGraphicAnnotation source, ICloningContext context)
		{
			context.CloneFields(source, this);
		}

		/// <summary>
		/// Gets or sets the <see cref="CompositeGraphic.CoordinateSystem"/>.
		/// </summary>
		/// <remarks>
		/// Setting the <see cref="CompositeGraphic.CoordinateSystem"/> property will recursively set the 
		/// <see cref="CompositeGraphic.CoordinateSystem"/> property for <i>all</i> <see cref="Graphic"/> 
		/// objects in the subtree.
		/// </remarks>
		public override sealed CoordinateSystem CoordinateSystem
		{
			get { return base.CoordinateSystem; }
			set { base.CoordinateSystem = value; }
		}

		/// <summary>
		/// Resets the <see cref="CompositeGraphic.CoordinateSystem"/>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <see cref="CompositeGraphic.ResetCoordinateSystem"/> will reset the <see cref="CompositeGraphic.CoordinateSystem"/>
		/// to what it was before the <see cref="CompositeGraphic.CoordinateSystem"/> was last set.
		/// </para>
		/// <para>
		/// Calling <see cref="CompositeGraphic.ResetCoordinateSystem"/> will recursively call
		/// <see cref="CompositeGraphic.ResetCoordinateSystem"/> on <i>all</i> <see cref="Graphic"/> 
		/// objects in the subtree.
		/// </para>
		/// </remarks>
		public override sealed void ResetCoordinateSystem()
		{
			base.ResetCoordinateSystem();
		}

		/// <summary>
		/// Gets or sets the color of this graphic annotation.
		/// </summary>
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

		/// <summary>
		/// Called when the <see cref="DicomGraphicAnnotation.Color"/> property changes.
		/// </summary>
		protected void OnColorChanged()
		{
			foreach (IGraphic graphic in base.Graphics)
			{
				if (graphic is PolyLineGraphic)
					((PolyLineGraphic) graphic).Color = _color;
				else if (graphic is VectorGraphic)
					((VectorGraphic) graphic).Color = _color;
				else if (graphic is CalloutGraphic)
					((CalloutGraphic) graphic).Color = _color;
			}
		}

		#region Static Graphic Creation Helpers

		private static IList<PointF> GetGraphicDataAsSourceCoordinates(RectangleF displayedArea, GraphicAnnotationSequenceItem.GraphicObjectSequenceItem graphicItem)
		{
			List<PointF> list;
			if (graphicItem.GraphicAnnotationUnits == GraphicAnnotationSequenceItem.GraphicAnnotationUnits.Display)
			{
				list = new List<PointF>(graphicItem.NumberOfGraphicPoints);
				foreach (PointF point in graphicItem.GraphicData)
					list.Add(GetPointInSourceCoordinates(displayedArea, point));
			}
			else
			{
				list = new List<PointF>(graphicItem.GraphicData);
			}
			return list.AsReadOnly();
		}

		private static PointF GetPointInSourceCoordinates(RectangleF displayedArea, PointF point)
		{
			return displayedArea.Location + new SizeF(displayedArea.Width*point.X, displayedArea.Height*point.Y);
		}

		private static IGraphic CreateInterpolated(IList<PointF> dataPoints)
		{
			// linear interpolation
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

		private static CalloutGraphic CreateCalloutText(RectangleF annotationBounds, RectangleF displayedArea, GraphicAnnotationSequenceItem.TextObjectSequenceItem textItem)
		{
			CalloutGraphic callout = new CalloutGraphic();
			callout.Text = textItem.UnformattedTextValue;

			if (textItem.AnchorPoint.HasValue)
			{
				PointF anchor = textItem.AnchorPoint.Value;
				if (textItem.AnchorPointAnnotationUnits == GraphicAnnotationSequenceItem.AnchorPointAnnotationUnits.Display)
					anchor = GetPointInSourceCoordinates(displayedArea, anchor);

				callout.EndPoint = anchor;
				callout.ShowArrow = true;
			}
			else
			{
				ILineSegmentGraphic calloutLine = GetCalloutLine(callout);
				if (calloutLine != null)
				{
					calloutLine.Visible = false;
				}
			}

			if (textItem.BoundingBoxTopLeftHandCorner.HasValue && textItem.BoundingBoxBottomRightHandCorner.HasValue)
			{
				PointF topLeft = textItem.BoundingBoxTopLeftHandCorner.Value;
				PointF bottomRight = textItem.BoundingBoxBottomRightHandCorner.Value;

				if(textItem.BoundingBoxAnnotationUnits == GraphicAnnotationSequenceItem.BoundingBoxAnnotationUnits.Display)
				{
					topLeft = GetPointInSourceCoordinates(displayedArea, topLeft);
					bottomRight = GetPointInSourceCoordinates(displayedArea, bottomRight);
				}

				// TODO: make the text variant - rotated as specified by bounding area - as well as justified as requested
				// RectangleF boundingBox = RectangleF.FromLTRB(topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);
				// boundingBox = RectangleUtilities.ConvertToPositiveRectangle(boundingBox);
				// boundingBox.Location = boundingBox.Location - new SizeF(1, 1);
				callout.Location = Vector.Midpoint(topLeft, bottomRight);
			}
			else
			{
				callout.CoordinateSystem = CoordinateSystem.Destination;
				callout.Location = annotationBounds.Location - new SizeF(30, 30);
				callout.ResetCoordinateSystem();
			}

			return callout;
		}

		private static ILineSegmentGraphic GetCalloutLine(CalloutGraphic callout)
		{
			return CollectionUtils.SelectFirst(callout.Graphics, delegate(IGraphic graphic) { return graphic is ILineSegmentGraphic; }) as ILineSegmentGraphic;
		}

		#endregion

		private class DicomGraphicAnnotationSerializer : GraphicAnnotationSerializer<DicomGraphicAnnotation>
		{
			protected override void Serialize(DicomGraphicAnnotation graphic, GraphicAnnotationSequenceItem serializationState) {}
		}
	}
}