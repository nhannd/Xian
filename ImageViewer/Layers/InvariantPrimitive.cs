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
		private PointF _anchorPoint;
		private event EventHandler<PointChangedEventArgs> _anchorPointChangedEvent;

		public InvariantPrimitive()
			: base(true)
		{
		}

		public event EventHandler<PointChangedEventArgs> AnchorPointChanged
		{
			add { _anchorPointChangedEvent += value; }
			remove { _anchorPointChangedEvent -= value; }
		}

		public PointF AnchorPoint
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
				{
					return _anchorPoint;
				}
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					return base.SpatialTransform.ConvertToDestination(_anchorPoint);
				}
			}
			set
			{
				if (FloatComparer.AreEqual(this.AnchorPoint, value))
					return;

				if (base.CoordinateSystem == CoordinateSystem.Source)
				{
					_anchorPoint = value;
				}
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					_anchorPoint = base.SpatialTransform.ConvertToSource(value);
				}

				EventsHelper.Fire(_anchorPointChangedEvent, this, new PointChangedEventArgs(this.AnchorPoint, this.CoordinateSystem));
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
