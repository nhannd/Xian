using System.Drawing;
using System.Drawing.Drawing2D;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A primitive arc graphic.
	/// </summary>
	/// <remarks>
	/// An arc is defined by a portion of the perimeter of an ellipse.
	/// The ellipse is defined by a bounding rectangle defined by the
	/// base class, <see cref="BoundableGraphic"/>.  The portion of the
	/// ellipse is defined by the <see cref="ArcPrimitive.StartAngle"/>
	/// and <see cref="ArcPrimitive.SweepAngle"/>.
	/// </remarks>
	public class ArcPrimitive : BoundableGraphic, IArcGraphic
	{
		private float _startAngle;
		private float _sweepAngle;

		/// <summary>
		/// Initializes a new instance of <see cref="ArcPrimitive"/>.
		/// </summary>
		public ArcPrimitive()
		{
			
		}

		/// <summary>
		/// Gets or sets the angle at which the arc begins.
		/// </summary>
		public float StartAngle
		{
			get
			{
				if (this.CoordinateSystem == CoordinateSystem.Source)
					return _startAngle;
				else
					return ArcPrimitive.ConvertStartAngleToDestination(_startAngle, this.SpatialTransform);
			}
			set
			{
				if (this.CoordinateSystem == CoordinateSystem.Source)
					_startAngle = value;
				else
					_startAngle = ArcPrimitive.ConvertStartAngleToSource(value, this.SpatialTransform);
			}
		}

		/// <summary>
		/// Gets or sets the angle that the arc sweeps out.
		/// </summary>
		public float SweepAngle
		{
			get
			{
				if (this.CoordinateSystem == CoordinateSystem.Source)
					return _sweepAngle;
				else
					return ArcPrimitive.ConvertSweepAngleToDestination(_sweepAngle, this.SpatialTransform);
			}
			set
			{
				if (this.CoordinateSystem == CoordinateSystem.Source)
					_sweepAngle = value;
				else
					_sweepAngle = ArcPrimitive.ConvertSweepAngleToSource(value, this.SpatialTransform);
			}
		}

		/// <summary>
		/// Performs a hit test on the <see cref="ArcPrimitive"/> at a given point.
		/// </summary>
		/// <param name="point">The mouse position in destination coordinates.</param>
		/// <returns>
		/// <b>True</b> if <paramref name="point"/> "hits" the <see cref="ArcPrimitive"/>,
		/// <b>false</b> otherwise.
		/// </returns>
		/// <remarks>
		/// A "hit" is defined as when the mouse position is <see cref="VectorGraphic.HitTestDistance"/>
		/// screen pixels away from any point on the arc.
		/// </remarks>
		public override bool HitTest(Point point)
		{
			this.CoordinateSystem = CoordinateSystem.Destination;
			bool result = HitTest(point, this.Rectangle, this.StartAngle, this.SweepAngle);
			this.ResetCoordinateSystem();

			return result;
		}

		/// <summary>
		/// Returns a value indicating whether the specified point is
		/// contained in the graphic.
		/// </summary>
		/// <param name="point"></param>
		/// <returns>Always returns <b>false</b>, since an arc cannot
		/// contain a point.</returns>
		public override bool Contains(PointF point)
		{
			return false;
		}

		internal static bool HitTest(
			PointF point, 
			RectangleF boundingBox, 
			float startAngle,
			float sweepAngle)
		{
			GraphicsPath path = new GraphicsPath();
			path.AddArc(RectangleUtilities.ConvertToPositiveRectangle(boundingBox), startAngle, sweepAngle);

			Pen pen = new Pen(Brushes.White, HitTestDistance);
			bool result = path.IsOutlineVisible(point, pen);

			path.Dispose();
			pen.Dispose();

			return result;
		}

		internal static float ConvertStartAngleToDestination(float startAngle, SpatialTransform transform)
		{
			if (transform.CumulativeFlipX)
				startAngle = -startAngle;

			if (transform.CumulativeFlipY)
				startAngle = 180 - startAngle;

			startAngle += transform.CumulativeRotationXY;

			if (startAngle < 0)
				startAngle += 360;

			return startAngle;
		}

		internal static float ConvertStartAngleToSource(float startAngle, SpatialTransform transform)
		{
			startAngle -= transform.CumulativeRotationXY;

			if (transform.CumulativeFlipY)
				startAngle = 180 - startAngle;

			if (transform.CumulativeFlipX)
				startAngle = -startAngle;

			if (startAngle < 0)
				startAngle += 360;

			return startAngle;
		}

		internal static float ConvertSweepAngleToDestination(float sweepAngle, SpatialTransform transform)
		{
			if ((transform.CumulativeFlipX && transform.CumulativeFlipY) ||
			    (!transform.CumulativeFlipX && !transform.CumulativeFlipY))
				return sweepAngle;
			else
				return -sweepAngle;
		}

		internal static float ConvertSweepAngleToSource(float sweepAngle, SpatialTransform transform)
		{
			if ((transform.CumulativeFlipX && transform.CumulativeFlipY) ||
				(!transform.CumulativeFlipX && !transform.CumulativeFlipY))
				return -sweepAngle;
			else
				return sweepAngle;
		}
	}
}
