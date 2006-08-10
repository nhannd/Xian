using System;
using System.Diagnostics;
using System.Drawing;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Layers
{
	/// <summary>
	/// Summary description for Rectangle.
	/// </summary>
	public class InvariantRectanglePrimitive : InvariantPrimitive
	{
		private RectangleF _Rectangle = new RectangleF(0,0,0,0);

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
					return new PointF(base.AnchorPoint.X + _Rectangle.Location.X, base.AnchorPoint.Y + _Rectangle.Location.Y);
				}
			}
		}

		public PointF InvariantTopLeft
		{
			get { return _Rectangle.Location; }
			set { _Rectangle.Location = value; }
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
				return new PointF(_Rectangle.X + _Rectangle.Width, _Rectangle.Y + _Rectangle.Height);
			}
			set
			{
				_Rectangle.Width = value.X - _Rectangle.X;
				_Rectangle.Height = value.Y - _Rectangle.Y;
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
					return _Rectangle.Width;
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
					return _Rectangle.Height;
				}
			}
		}

		public override bool HitTest(XMouseEventArgs e)
		{
			throw new NotImplementedException();
		}

	}
}
