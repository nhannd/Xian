using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Mathematics;
using ClearCanvas.ImageViewer.DynamicOverlays;

namespace ClearCanvas.ImageViewer.Layers
{
	/// <summary>
	/// Summary description for Line.
	/// </summary>
	public class LinePrimitive : Graphic
	{
		private PointF m_SrcPt1;
		private PointF m_SrcPt2;
		private event EventHandler<PointChangedEventArgs> m_Pt1ChangedEvent;
		private event EventHandler<PointChangedEventArgs> m_Pt2ChangedEvent;

		public LinePrimitive() :
			base(true)
		{
		}

		public event EventHandler<PointChangedEventArgs> Pt1Changed
		{
			add { m_Pt1ChangedEvent += value; }
			remove { m_Pt1ChangedEvent -= value; }
		}

		public event EventHandler<PointChangedEventArgs> Pt2Changed
		{
			add { m_Pt2ChangedEvent += value; }
			remove { m_Pt2ChangedEvent -= value; }
		}

		public PointF Pt1
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
					return m_SrcPt1;
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					return base.SpatialTransform.ConvertToDestination(m_SrcPt1);
				}
			}
			set
			{
				if (FloatComparer.AreEqual(this.Pt1, value))
					return;

				if (base.CoordinateSystem == CoordinateSystem.Source)
					m_SrcPt1 = value;
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					m_SrcPt1 = base.SpatialTransform.ConvertToSource(value);
				}

				EventsHelper.Fire(m_Pt1ChangedEvent, this, new PointChangedEventArgs(this.Pt1, this.CoordinateSystem));
			}
		}

		public PointF Pt2
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
					return m_SrcPt2;
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					return base.SpatialTransform.ConvertToDestination(m_SrcPt2);
				}
			}
			set
			{
				if (FloatComparer.AreEqual(this.Pt2, value))
					return;

				if (base.CoordinateSystem == CoordinateSystem.Source)
					m_SrcPt2 = value;
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					m_SrcPt2 = base.SpatialTransform.ConvertToSource(value);
				}

				EventsHelper.Fire(m_Pt1ChangedEvent, this, new PointChangedEventArgs(this.Pt2, this.CoordinateSystem));
			}
		}

		public override bool HitTest(XMouseEventArgs e)
		{
			double distance;
			PointF ptNearest = new PointF(0, 0);
			PointF ptMouse = this.SpatialTransform.ConvertToSource(new PointF(e.X, e.Y));

			this.CoordinateSystem = CoordinateSystem.Source;
			distance = Vector.DistanceFromPointToLine(ptMouse, this.Pt1, this.Pt2, ref ptNearest);
			this.ResetCoordinateSystem();

			// Convert hit test distance to source coordinates.  This is a bit of a hack,
			// since we're not accounting for the case in which scaleX and scaleY are different.
			// But, since this is just a hit test, it's inconsequential, and we don't
			// want to be doing any complicated processing here since this method is called
			// whenever the mouse moves.
			float sourceHitTestDistance = InteractiveGraphic.HitTestDistance / this.SpatialTransform.Scale;

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
