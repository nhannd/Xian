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
		private RectangleF m_Rectangle = new RectangleF(0,0,0,0);

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
					return new PointF(base.AnchorPoint.X + m_Rectangle.Location.X, base.AnchorPoint.Y + m_Rectangle.Location.Y);
				}
			}
		}

		public PointF InvariantTopLeft
		{
			get { return m_Rectangle.Location; }
			set { m_Rectangle.Location = value; }
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
				return new PointF(m_Rectangle.X + m_Rectangle.Width, m_Rectangle.Y + m_Rectangle.Height);
			}
			set
			{
				m_Rectangle.Width = value.X - m_Rectangle.X;
				m_Rectangle.Height = value.Y - m_Rectangle.Y;
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
					return m_Rectangle.Width;
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
					return m_Rectangle.Height;
				}
			}
		}

		public override bool HitTest(XMouseEventArgs e)
		{
			throw new NotImplementedException();
		}

	}
}
