using System;
using System.Diagnostics;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.DynamicOverlays;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Layers
{
	public class RectanglePrimitive : Graphic
	{
		private RectangleF _rectangle = new RectangleF(0,0,0,0);
		private event EventHandler<PointChangedEventArgs> _topLeftChangedEvent;
		private event EventHandler<PointChangedEventArgs> _bottomRightChangedEvent;

		public RectanglePrimitive() 
			: base(true)
		{
		}

		public PointF TopLeft
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
					return _rectangle.Location;
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					return base.SpatialTransform.ConvertToDestination(_rectangle.Location);
				}
			}
			set
			{
				if (FloatComparer.AreEqual(this.TopLeft, value))
					return;

				if (base.CoordinateSystem == CoordinateSystem.Source)
				{
					_rectangle.Width = _rectangle.Right - value.X;
					_rectangle.Height = _rectangle.Bottom - value.Y;
					_rectangle.Location = value;
				}
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");

					_rectangle.Width = _rectangle.Right - base.SpatialTransform.ConvertToSource(value).X;
					_rectangle.Height = _rectangle.Bottom - base.SpatialTransform.ConvertToSource(value).Y;
					_rectangle.Location = base.SpatialTransform.ConvertToSource(value);
				}

				EventsHelper.Fire(_topLeftChangedEvent, this, new PointChangedEventArgs(this.TopLeft, this.CoordinateSystem));
			}
		}

		public PointF BottomRight
		{
			get
			{
				PointF bottomRight = new PointF(_rectangle.Right, _rectangle.Bottom);

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
					_rectangle.Width = value.X - _rectangle.X;
					_rectangle.Height = value.Y - _rectangle.Y;
				}
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					PointF pointSrc = base.SpatialTransform.ConvertToSource(value);

					_rectangle.Width = pointSrc.X - _rectangle.X;
					_rectangle.Height = pointSrc.Y - _rectangle.Y;
				}

				EventsHelper.Fire(_bottomRightChangedEvent, this, new PointChangedEventArgs(this.BottomRight, this.CoordinateSystem));
			}
		}

		public float Width
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
				{
					return _rectangle.Width;
				}
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");

					PointF topLeft = base.SpatialTransform.ConvertToDestination(_rectangle.Location);
					PointF bottomRight = base.SpatialTransform.ConvertToDestination(new PointF(_rectangle.Right, _rectangle.Bottom));

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
					return _rectangle.Height;
				}
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");

					PointF topLeft = base.SpatialTransform.ConvertToDestination(_rectangle.Location);
					PointF bottomRight = base.SpatialTransform.ConvertToDestination(new PointF(_rectangle.Right, _rectangle.Bottom));

					return bottomRight.Y - topLeft.Y;
				}
			}
		}

		public event EventHandler<PointChangedEventArgs> TopLeftChanged
		{
			add { _topLeftChangedEvent += value; }
			remove { _topLeftChangedEvent -= value; }
		}

		public event EventHandler<PointChangedEventArgs> BottomRightChanged
		{
			add { _bottomRightChangedEvent += value; }
			remove { _bottomRightChangedEvent -= value; }
		}

		public override bool HitTest(Point point)
		{
			double distance;
			PointF ptNearest = new PointF(0, 0);
			PointF ptMouse = this.SpatialTransform.ConvertToSource(new PointF(point.X, point.Y));

			this.CoordinateSystem = CoordinateSystem.Source;

			PointF ptTopLeft = this.TopLeft;
			PointF ptBottomRight = this.BottomRight;
			PointF ptTopRight = new PointF(ptBottomRight.X, ptTopLeft.Y);
			PointF ptBottomLeft = new PointF(ptTopLeft.X, ptBottomRight.Y);

			this.ResetCoordinateSystem();

			// TODO: Hit test distance should be in source coords

			distance = Vector.DistanceFromPointToLine(ptMouse, ptTopLeft, ptTopRight, ref ptNearest);

			if (distance < InteractiveGraphic._hitTestDistance)
				return true;

			distance = Vector.DistanceFromPointToLine(ptMouse, ptTopRight, ptBottomRight, ref ptNearest);

			if (distance < InteractiveGraphic._hitTestDistance)
				return true;

			distance = Vector.DistanceFromPointToLine(ptMouse, ptBottomRight, ptBottomLeft, ref ptNearest);

			if (distance < InteractiveGraphic._hitTestDistance)
				return true;

			distance = Vector.DistanceFromPointToLine(ptMouse, ptBottomLeft, ptTopLeft, ref ptNearest);

			if (distance < InteractiveGraphic._hitTestDistance)
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
				return _rectangle.Contains(point);
			else
			{
				Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
				return _rectangle.Contains(base.SpatialTransform.ConvertToSource(point));
			}
		}
	}
}
