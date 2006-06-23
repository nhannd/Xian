using System;
using System.Diagnostics;
using System.Drawing;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Layers
{
	/// <summary>
	/// Summary description for GraphicElement.
	/// </summary>
	public abstract class InvariantPrimitive : Graphic
	{
		private PointF m_AnchorPoint;
		private event EventHandler<PointChangedEventArgs> m_AnchorPointChangedEvent;

		public InvariantPrimitive()
			: base(true)
		{
		}

		public event EventHandler<PointChangedEventArgs> AnchorPointChanged
		{
			add { m_AnchorPointChangedEvent += value; }
			remove { m_AnchorPointChangedEvent -= value; }
		}

		public PointF AnchorPoint
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
				{
					return m_AnchorPoint;
				}
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					return base.SpatialTransform.ConvertToDestination(m_AnchorPoint);
				}
			}
			set
			{
				if (FloatComparer.AreEqual(this.AnchorPoint, value))
					return;

				if (base.CoordinateSystem == CoordinateSystem.Source)
				{
					m_AnchorPoint = value;
				}
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					m_AnchorPoint = base.SpatialTransform.ConvertToSource(value);
				}

				EventsHelper.Fire(m_AnchorPointChangedEvent, this, new PointChangedEventArgs(this.AnchorPoint, this.CoordinateSystem));
			}
		}

		public override void Move(SizeF delta)
		{
#if MONO
			Size del = new Size((int)delta.Width, (int)delta.Height);
			this.AnchorPoint += del;
#else
			this.AnchorPoint += delta;
#endif
		}

	}
}
