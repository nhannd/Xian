using System;
using System.Diagnostics;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A <see cref="VectorGraphic"/> whose size in destination coordinates is invariant
	/// under zoom.
	/// </summary>
	/// <remarks>
	/// Sometimes it is desirable to have a <see cref="VectorGraphic"/> whose
	/// <i>size</i> is invariant under zoom, but whose position is not.  A good example
	/// are the <see cref="ControlPoint"/> objects on measurement tools that allow a 
	/// user to stretch and resize the measurement.  They are anchored to a certain
	/// point on the underlying image so that when zoomed, the control point appears
	/// to "move" with the zoom of the image, but their size
	/// in screen pixels remains the same.
	/// </remarks>
	public abstract class InvariantPrimitive : VectorGraphic
	{
		private PointF _anchorPoint;
		private event EventHandler<PointChangedEventArgs> _anchorPointChangedEvent;

		/// <summary>
		/// Initializes a new instance of <see cref="InvariantPrimitive"/>.
		/// </summary>
		public InvariantPrimitive()
		{
		}

		/// <summary>
		/// Occurs when the <see cref="AnchorPoint"/> property has changed.
		/// </summary>
		public event EventHandler<PointChangedEventArgs> AnchorPointChanged
		{
			add { _anchorPointChangedEvent += value; }
			remove { _anchorPointChangedEvent -= value; }
		}

		/// <summary>
		/// The point in the coordinate system of the parent <see cref="IGraphic"/> where the
		/// <see cref="InvariantPrimitive"/> is anchored.
		/// </summary>
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

				EventsHelper.Fire(_anchorPointChangedEvent, this, new PointChangedEventArgs(this.AnchorPoint));
			}
		}

		/// <summary>
		/// Moves the <see cref="Graphic"/> by a specified delta.
		/// </summary>
		/// <param name="delta">The distance to move.</param>
		/// <remarks>
		/// Depending on the value of <see cref="Graphic.CoordinateSystem"/>,
		/// <paramref name="delta"/> will be interpreted in either source
		/// or destination coordinates.
		/// </remarks>
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
