using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.DynamicOverlays;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Layers
{
	public class LinePrimitive : Graphic
	{
		private PointF _srcPt1;
		private PointF _srcPt2;
		private event EventHandler<PointChangedEventArgs> _pt1ChangedEvent;
		private event EventHandler<PointChangedEventArgs> _pt2ChangedEvent;

		public LinePrimitive() :
			base(true)
		{
		}

		public event EventHandler<PointChangedEventArgs> Pt1Changed
		{
			add { _pt1ChangedEvent += value; }
			remove { _pt1ChangedEvent -= value; }
		}

		public event EventHandler<PointChangedEventArgs> Pt2Changed
		{
			add { _pt2ChangedEvent += value; }
			remove { _pt2ChangedEvent -= value; }
		}

		public PointF Pt1
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
					return _srcPt1;
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					return base.SpatialTransform.ConvertToDestination(_srcPt1);
				}
			}
			set
			{
				if (FloatComparer.AreEqual(this.Pt1, value))
					return;

				if (base.CoordinateSystem == CoordinateSystem.Source)
					_srcPt1 = value;
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					_srcPt1 = base.SpatialTransform.ConvertToSource(value);
				}

				EventsHelper.Fire(_pt1ChangedEvent, this, new PointChangedEventArgs(this.Pt1, this.CoordinateSystem));
			}
		}

		public PointF Pt2
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
					return _srcPt2;
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					return base.SpatialTransform.ConvertToDestination(_srcPt2);
				}
			}
			set
			{
				if (FloatComparer.AreEqual(this.Pt2, value))
					return;

				if (base.CoordinateSystem == CoordinateSystem.Source)
					_srcPt2 = value;
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					_srcPt2 = base.SpatialTransform.ConvertToSource(value);
				}

				EventsHelper.Fire(_pt1ChangedEvent, this, new PointChangedEventArgs(this.Pt2, this.CoordinateSystem));
			}
		}

		public override bool HitTest(Point point)
		{
			double distance;
			PointF ptNearest = new PointF(0, 0);
			PointF ptMouse = this.SpatialTransform.ConvertToSource(new PointF(point.X, point.Y));

			this.CoordinateSystem = CoordinateSystem.Source;
			distance = Vector.DistanceFromPointToLine(ptMouse, this.Pt1, this.Pt2, ref ptNearest);
			this.ResetCoordinateSystem();

			// Convert hit test distance to source coordinates.  This is a bit of a hack,
			// since we're not accounting for the case in which scaleX and scaleY are different.
			// But, since this is just a hit test, it's inconsequential, and we don't
			// want to be doing any complicated processing here since this method is called
			// whenever the mouse moves.
			float sourceHitTestDistance = InteractiveGraphic._hitTestDistance / this.SpatialTransform.Scale;

			if (distance < sourceHitTestDistance)
				return true;
			else
				return false;
		}

		public override void Move(SizeF delta)
		{
#if MONO
			Size del = new Size((int)delta.Width, (int)delta.Height);
			this.Pt1 += del;
			this.Pt2 += del;
#else
			this.Pt1 += delta;
			this.Pt2 += delta;
#endif
		}
	}
}
