using System;
using System.Diagnostics;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Mathematics;
using ClearCanvas.ImageViewer.DynamicOverlays;

namespace ClearCanvas.ImageViewer.Layers
{
	/// <summary>
	/// Summary description for Rectangle.
	/// </summary>
	public class RectanglePrimitive : Graphic
	{
		private RectangleF _Rectangle = new RectangleF(0,0,0,0);
		private event EventHandler<PointChangedEventArgs> _TopLeftChangedEvent;
		private event EventHandler<PointChangedEventArgs> _BottomRightChangedEvent;

		public RectanglePrimitive() 
			: base(true)
		{
		}

		public PointF TopLeft
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
					return _Rectangle.Location;
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					return base.SpatialTransform.ConvertToDestination(_Rectangle.Location);
				}
			}
			set
			{
				if (FloatComparer.AreEqual(this.TopLeft, value))
					return;

				if (base.CoordinateSystem == CoordinateSystem.Source)
				{
					_Rectangle.Width = _Rectangle.Right - value.X;
					_Rectangle.Height = _Rectangle.Bottom - value.Y;
					_Rectangle.Location = value;
				}
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");

					_Rectangle.Width = _Rectangle.Right - base.SpatialTransform.ConvertToSource(value).X;
					_Rectangle.Height = _Rectangle.Bottom - base.SpatialTransform.ConvertToSource(value).Y;
					_Rectangle.Location = base.SpatialTransform.ConvertToSource(value);
				}

				EventsHelper.Fire(_TopLeftChangedEvent, this, new PointChangedEventArgs(this.TopLeft, this.CoordinateSystem));
			}
		}

		public PointF BottomRight
		{
			get
			{
				PointF bottomRight = new PointF(_Rectangle.Right, _Rectangle.Bottom);

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
					_Rectangle.Width = value.X - _Rectangle.X;
					_Rectangle.Height = value.Y - _Rectangle.Y;
				}
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					PointF pointSrc = base.SpatialTransform.ConvertToSource(value);

					_Rectangle.Width = pointSrc.X - _Rectangle.X;
					_Rectangle.Height = pointSrc.Y - _Rectangle.Y;
				}

				EventsHelper.Fire(_BottomRightChangedEvent, this, new PointChangedEventArgs(this.BottomRight, this.CoordinateSystem));
			}
		}

		public float Width
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
				{
					return _Rectangle.Width;
				}
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");

					PointF topLeft = base.SpatialTransform.ConvertToDestination(_Rectangle.Location);
					PointF bottomRight = base.SpatialTransform.ConvertToDestination(new PointF(_Rectangle.Right, _Rectangle.Bottom));

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
					return _Rectangle.Height;
				}
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");

					PointF topLeft = base.SpatialTransform.ConvertToDestination(_Rectangle.Location);
					PointF bottomRight = base.SpatialTransform.ConvertToDestination(new PointF(_Rectangle.Right, _Rectangle.Bottom));

					return bottomRight.Y - topLeft.Y;
				}
			}
		}

		public event EventHandler<PointChangedEventArgs> TopLeftChanged
		{
			add { _TopLeftChangedEvent += value; }
			remove { _TopLeftChangedEvent -= value; }
		}

		public event EventHandler<PointChangedEventArgs> BottomRightChanged
		{
			add { _BottomRightChangedEvent += value; }
			remove { _BottomRightChangedEvent -= value; }
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
				return _Rectangle.Contains(point);
			else
			{
				Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
				return _Rectangle.Contains(base.SpatialTransform.ConvertToSource(point));
			}
		}
	}
}
