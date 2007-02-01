using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Graphics
{
	public class LinePrimitive : VectorGraphic
	{
		private PointF _srcPt1;
		private PointF _srcPt2;
		private event EventHandler<PointChangedEventArgs> _pt1ChangedEvent;
		private event EventHandler<PointChangedEventArgs> _pt2ChangedEvent;

		public LinePrimitive()
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

			// Always do the hit test in destination coordinates since we want the
			// "activation distance" to be the same irrespective of the zoom
			this.CoordinateSystem = CoordinateSystem.Destination;
			distance = Vector.DistanceFromPointToLine(point, this.Pt1, this.Pt2, ref ptNearest);
			this.ResetCoordinateSystem();

			if (distance < InteractiveGraphic.HitTestDistance)
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
