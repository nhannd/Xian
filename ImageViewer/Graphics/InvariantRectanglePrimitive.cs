using System;
using System.Diagnostics;
using System.Drawing;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Graphics
{
	public class InvariantRectanglePrimitive : InvariantPrimitive
	{
		private RectangleF _rectangle = new RectangleF(0,0,0,0);

		public InvariantRectanglePrimitive()
		{
		}

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
					return new PointF(base.AnchorPoint.X + _rectangle.Location.X, base.AnchorPoint.Y + _rectangle.Location.Y);
				}
			}
		}

		public PointF InvariantTopLeft
		{
			get { return _rectangle.Location; }
			set { _rectangle.Location = value; }
		}

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
					return new PointF(base.AnchorPoint.X + this.InvariantBottomRight.X, base.AnchorPoint.Y + this.InvariantBottomRight.Y);
				}
			}
		}

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

		public override bool HitTest(Point point)
		{
			throw new NotImplementedException();
		}

	}
}
