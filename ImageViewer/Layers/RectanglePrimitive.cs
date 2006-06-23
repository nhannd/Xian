using System;
using System.Diagnostics;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Mathematics;
using ClearCanvas.Workstation.Model.DynamicOverlays;

namespace ClearCanvas.Workstation.Model.Layers
{
	/// <summary>
	/// Summary description for Rectangle.
	/// </summary>
	public class RectanglePrimitive : Graphic
	{
		private RectangleF m_Rectangle = new RectangleF(0,0,0,0);
		private event EventHandler<PointChangedEventArgs> m_TopLeftChangedEvent;
		private event EventHandler<PointChangedEventArgs> m_BottomRightChangedEvent;

		public RectanglePrimitive() 
			: base(true)
		{
		}

		public PointF TopLeft
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
					return m_Rectangle.Location;
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					return base.SpatialTransform.ConvertToDestination(m_Rectangle.Location);
				}
			}
			set
			{
				if (FloatComparer.AreEqual(this.TopLeft, value))
					return;

				if (base.CoordinateSystem == CoordinateSystem.Source)
				{
					m_Rectangle.Width = m_Rectangle.Right - value.X;
					m_Rectangle.Height = m_Rectangle.Bottom - value.Y;
					m_Rectangle.Location = value;
				}
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");

					m_Rectangle.Width = m_Rectangle.Right - base.SpatialTransform.ConvertToSource(value).X;
					m_Rectangle.Height = m_Rectangle.Bottom - base.SpatialTransform.ConvertToSource(value).Y;
					m_Rectangle.Location = base.SpatialTransform.ConvertToSource(value);
				}

				EventsHelper.Fire(m_TopLeftChangedEvent, this, new PointChangedEventArgs(this.TopLeft, this.CoordinateSystem));
			}
		}

		public PointF BottomRight
		{
			get
			{
				PointF bottomRight = new PointF(m_Rectangle.Right, m_Rectangle.Bottom);

				if (base.CoordinateSystem == CoordinateSystem.Source)
					return bottomRight;
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					return base.SpatialTransform.ConvertToDestination(bottomRight);
				}
			}
			set
			{
				if (FloatComparer.AreEqual(this.BottomRight, value))
					return;

				if (base.CoordinateSystem == CoordinateSystem.Source)
				{
					m_Rectangle.Width = value.X - m_Rectangle.X;
					m_Rectangle.Height = value.Y - m_Rectangle.Y;
				}
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					PointF pointSrc = base.SpatialTransform.ConvertToSource(value);

					m_Rectangle.Width = pointSrc.X - m_Rectangle.X;
					m_Rectangle.Height = pointSrc.Y - m_Rectangle.Y;
				}

				EventsHelper.Fire(m_BottomRightChangedEvent, this, new PointChangedEventArgs(this.BottomRight, this.CoordinateSystem));
			}
		}

		public float Width
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
				{
					return m_Rectangle.Width;
				}
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");

					PointF topLeft = base.SpatialTransform.ConvertToDestination(m_Rectangle.Location);
					PointF bottomRight = base.SpatialTransform.ConvertToDestination(new PointF(m_Rectangle.Right, m_Rectangle.Bottom));

					return bottomRight.X - topLeft.X;
				}
			}
		}

		public float Height
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
				{
					return m_Rectangle.Height;
				}
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");

					PointF topLeft = base.SpatialTransform.ConvertToDestination(m_Rectangle.Location);
					PointF bottomRight = base.SpatialTransform.ConvertToDestination(new PointF(m_Rectangle.Right, m_Rectangle.Bottom));

					return bottomRight.Y - topLeft.Y;
				}
			}
		}

		public event EventHandler<PointChangedEventArgs> TopLeftChanged
		{
			add { m_TopLeftChangedEvent += value; }
			remove { m_TopLeftChangedEvent -= value; }
		}

		public event EventHandler<PointChangedEventArgs> BottomRightChanged
		{
			add { m_BottomRightChangedEvent += value; }
			remove { m_BottomRightChangedEvent -= value; }
		}

		public override bool HitTest(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			double distance;
			PointF ptNearest = new PointF(0, 0);
			PointF ptMouse = this.SpatialTransform.ConvertToSource(new PointF(e.X, e.Y));

			this.CoordinateSystem = CoordinateSystem.Source;

			PointF ptTopLeft = this.TopLeft;
			PointF ptBottomRight = this.BottomRight;
			PointF ptTopRight = new PointF(ptBottomRight.X, ptTopLeft.Y);
			PointF ptBottomLeft = new PointF(ptTopLeft.X, ptBottomRight.Y);

			this.ResetCoordinateSystem();

			// TODO: Hit test distance should be in source coords

			distance = Vector.DistanceFromPointToLine(ptMouse, ptTopLeft, ptTopRight, ref ptNearest);

			if (distance < InteractiveGraphic.HitTestDistance)
				return true;

			distance = Vector.DistanceFromPointToLine(ptMouse, ptTopRight, ptBottomRight, ref ptNearest);

			if (distance < InteractiveGraphic.HitTestDistance)
				return true;

			distance = Vector.DistanceFromPointToLine(ptMouse, ptBottomRight, ptBottomLeft, ref ptNearest);

			if (distance < InteractiveGraphic.HitTestDistance)
				return true;

			distance = Vector.DistanceFromPointToLine(ptMouse, ptBottomLeft, ptTopLeft, ref ptNearest);

			if (distance < InteractiveGraphic.HitTestDistance)
				return true;

			return false;
		}

		public override void Move(SizeF delta)
		{
#if MONO
			Size del = new Size((int)delta.Width, (int)delta.Height);
			this.TopLeft += del;
			this.BottomRight += del;
#else
			this.TopLeft += delta;
			this.BottomRight += delta;
#endif
		}

		public bool Contains(PointF point)
		{
			if (base.CoordinateSystem == CoordinateSystem.Source)
				return m_Rectangle.Contains(point);
			else
			{
				Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
				return m_Rectangle.Contains(base.SpatialTransform.ConvertToSource(point));
			}
		}
	}
}
