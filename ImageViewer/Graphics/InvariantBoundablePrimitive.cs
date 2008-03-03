using System.Drawing;
using ClearCanvas.Common;
using System.Drawing.Drawing2D;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// An <see cref="InvariantPrimitive"/> that can be described by a
	/// rectangular bounding box.
	/// </summary>
	public abstract class InvariantBoundablePrimitive : InvariantPrimitive, IBoundableGraphic
	{
		private RectangleF _rectangle = new RectangleF(0,0,0,0);

		/// <summary>
		/// Gets the top left corner of the rectangle in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		public PointF TopLeft
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");

					this.CoordinateSystem = CoordinateSystem.Destination;
					PointF topLeft = this.SpatialTransform.ConvertToSource(this.TopLeft);
					this.ResetCoordinateSystem();

					return topLeft;
				}
				else
				{
					Matrix m = new Matrix();
					m.Rotate(this.SpatialTransform.CumulativeRotationXY);
					PointF[] pt = { this.InvariantTopLeft };
					m.TransformPoints(pt);
					m.Dispose();

					return new PointF(base.AnchorPoint.X + pt[0].X, base.AnchorPoint.Y + pt[0].Y);
				}
			}
		}

		/// <summary>
		/// Gets or sets the top left corner of the rectangle in <i>screen</i> pixels relative to 
		/// the anchor point.
		/// </summary>
		/// <remarks>
		/// <see cref="InvariantTopLeft"/> is different from <see cref="TopLeft"/> in that
		/// it is the top left corner of the rectangle in screen pixels where (0,0)
		/// is the anchor point. For example, if you wanted an
		/// invariant square of size 9x9 screen pixels and the anchor point is in the middle
		/// of the square, <see cref="InvariantTopLeft"/> would be (-4,-4) and 
		/// <see cref="InvariantBottomRight"/> would be (4,4).
		/// </remarks>
		public PointF InvariantTopLeft
		{
			get { return _rectangle.Location; }
			set { _rectangle.Location = value; }
		}

		/// <summary>
		/// Gets the bottom right corner of the rectangle in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		public PointF BottomRight
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");

					this.CoordinateSystem = CoordinateSystem.Destination;
					PointF bottomRight = this.SpatialTransform.ConvertToSource(this.BottomRight);
					this.ResetCoordinateSystem();

					return bottomRight;
				}
				else
				{
					Matrix m = new Matrix();
					m.Rotate(this.SpatialTransform.CumulativeRotationXY);
					PointF[] pt = { this.InvariantBottomRight };
					m.TransformPoints(pt);
					m.Dispose();

					return new PointF(base.AnchorPoint.X + pt[0].X, base.AnchorPoint.Y + pt[0].Y);
				}
			}
		}

		/// <summary>
		/// Gets or sets the bottom right corner of the rectangle in <i>screen</i> pixels relative to 
		/// the anchor point.
		/// </summary>
		/// <remarks>
		/// <see cref="InvariantBottomRight"/> is different from <see cref="BottomRight"/> in that
		/// it is the bottom right corner of the rectangle in screen pixels where (0,0)
		/// is the anchor point. For example, if you wanted an
		/// invariant square of size 9x9 screen pixels and the anchor point is in the middle
		/// of the square, <see cref="InvariantTopLeft"/> would be (-4,-4) and 
		/// <see cref="InvariantBottomRight"/> would be (4,4).
		/// </remarks>
		public PointF InvariantBottomRight
		{
			get
			{
				return new PointF(_rectangle.X + _rectangle.Width, _rectangle.Y + _rectangle.Height);
			}
			set
			{
				_rectangle.Width = value.X - _rectangle.X;
				_rectangle.Height = value.Y - _rectangle.Y;
			}
		}

		/// <summary>
		/// Gets the width of the rectangle in either source or destination pixels.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination pixels.
		/// </remarks>
		public float Width
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
				{
					return this.BottomRight.X - this.TopLeft.X;
				}
				else
				{
					return _rectangle.Width;
				}
			}
		}

		/// <summary>
		/// Gets the height of the rectangle in either source or destination pixels.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		public float Height
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
				{
					return this.BottomRight.Y - this.TopLeft.Y;
				}
				else
				{
					return _rectangle.Height;
				}
			}
		}

		/// <summary>
		/// Gets the rectangle that defines a <see cref="BoundableGraphic"/>.
		/// </summary>
		protected RectangleF Rectangle
		{
			get
			{
				return new RectangleF(this.TopLeft.X, this.TopLeft.Y, this.Width, this.Height);
			}
		}

	}
}