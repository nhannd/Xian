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
			get { return _startAngle; }
			set { _startAngle = value; }
		}

		/// <summary>
		/// Gets or sets the angle that the arc sweeps out.
		/// </summary>
		public float SweepAngle
		{
			get { return _sweepAngle; }
			set { _sweepAngle = value; }
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
	}
}
