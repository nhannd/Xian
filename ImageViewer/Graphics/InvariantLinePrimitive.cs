using System;
using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A linear <see cref="InvariantPrimitive"/>.
	/// </summary>
	/// <remarks>
	/// <para>This primitive graphic defines a line whose position can be fixed to the
	/// source coordinate system and whose length will be fixed relative to the
	/// destination coordinate system.</para>
	/// <para>The <see cref="InvariantLinePrimitive.AnchorPoint"/> defines the point
	/// that is affixed to the source coordinate system, and the <see cref="InvariantLinePrimitive.InvariantTopLeft"/>
	/// and <see cref="InvariantLinePrimitive.InvariantBottomRight"/> properties define the length
	/// and orientation of the line.</para>
	/// </remarks>
	[Cloneable(true)]
	public class InvariantLinePrimitive : InvariantBoundablePrimitive, ILineSegmentGraphic
	{
		/// <summary>
		/// Constructs a new invariant line primitive.
		/// </summary>
		public InvariantLinePrimitive() {}

		/// <summary>
		/// Performs a hit test on the <see cref="Graphic"/> at a given point.
		/// </summary>
		/// <param name="point">The mouse position in destination coordinates.</param>
		/// <returns>
		/// <b>True</b> if <paramref name="point"/> "hits" the <see cref="InvariantLinePrimitive"/>,
		/// <b>false</b> otherwise.
		/// </returns>
		public override bool HitTest(Point point)
		{
			this.CoordinateSystem = CoordinateSystem.Destination;
			try
			{
				PointF output = new PointF();
				double distance = Vector.DistanceFromPointToLine(point, this.TopLeft, this.BottomRight, ref output);
				return distance < HitTestDistance;
			}
			finally
			{
				this.ResetCoordinateSystem();
			}
		}

		public override PointF GetClosestPoint(PointF point)
		{
			PointF result = PointF.Empty;
			RectangleF rect = this.Rectangle;
			Vector.DistanceFromPointToLine(point, new PointF(rect.Left, rect.Top), new PointF(rect.Right, rect.Bottom), ref result);
			return result;
		}

		public override bool Contains(Point point)
		{
			return false;
		}

		/// <summary>
		/// The endpoint of the line as specified by <see cref="InvariantLinePrimitive.TopLeft"/> in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		PointF ILineSegmentGraphic.Pt1
		{
			get { return this.TopLeft; }
			set { throw new NotSupportedException(); }
		}

		/// <summary>
		/// The endpoint of the line as specified by <see cref="InvariantLinePrimitive.BottomRight"/> in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		PointF ILineSegmentGraphic.Pt2
		{
			get { return this.BottomRight; }
			set { throw new NotSupportedException(); }
		}

		event EventHandler<PointChangedEventArgs> ILineSegmentGraphic.Pt1Changed
		{
			add { }
			remove { }
		}

		event EventHandler<PointChangedEventArgs> ILineSegmentGraphic.Pt2Changed
		{
			add { }
			remove { }
		}
	}
}