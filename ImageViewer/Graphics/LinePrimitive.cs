using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A primitive line graphic.
	/// </summary>
	public class LinePrimitive : VectorGraphic
	{
		#region Private fields
		
		private PointF _srcPt1;
		private PointF _srcPt2;
		private event EventHandler<PointChangedEventArgs> _pt1ChangedEvent;
		private event EventHandler<PointChangedEventArgs> _pt2ChangedEvent;
		
		#endregion

		/// <summary>
		/// Initializes a new instance of <see cref="LinePrimitive"/>.
		/// </summary>
		public LinePrimitive()
		{
		}

		/// <summary>
		/// The start point of the line in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
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

				EventsHelper.Fire(_pt1ChangedEvent, this, new PointChangedEventArgs(this.Pt1));
			}
		}

		/// <summary>
		/// The end point of the line in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
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

				EventsHelper.Fire(_pt1ChangedEvent, this, new PointChangedEventArgs(this.Pt2));
			}
		}

		/// <summary>
		/// Occurs when <see cref="Pt1"/> has changed.
		/// </summary>
		public event EventHandler<PointChangedEventArgs> Pt1Changed
		{
			add { _pt1ChangedEvent += value; }
			remove { _pt1ChangedEvent -= value; }
		}

		/// <summary>
		/// Occurs when <see cref="Pt2"/> has changed.
		/// </summary>
		public event EventHandler<PointChangedEventArgs> Pt2Changed
		{
			add { _pt2ChangedEvent += value; }
			remove { _pt2ChangedEvent -= value; }
		}

		/// <summary>
		/// Performs a hit test on the <see cref="LinePrimitive"/> at a given point.
		/// </summary>
		/// <param name="point">The mouse position in destination coordinates.</param>
		/// <returns>
		/// <b>True</b> if <paramref name="point"/> "hits" the <see cref="LinePrimitive"/>,
		/// <b>false</b> otherwise.
		/// </returns>
		/// <remarks>
		/// A "hit" is defined as when the mouse position is <see cref="VectorGraphic.HitTestDistance"/>
		/// screen pixels away from any point on the line.
		/// </remarks>
		public override bool HitTest(Point point)
		{
			double distance;
			PointF ptNearest = new PointF(0, 0);

			// Always do the hit test in destination coordinates since we want the
			// "activation distance" to be the same irrespective of the zoom
			this.CoordinateSystem = CoordinateSystem.Destination;
			distance = Vector.DistanceFromPointToLine(point, this.Pt1, this.Pt2, ref ptNearest);
			this.ResetCoordinateSystem();

			if (distance < VectorGraphic.HitTestDistance)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Moves the <see cref="LinePrimitive"/> by a specified delta.
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
			this.Pt1 += del;
			this.Pt2 += del;
#else
			this.Pt1 += delta;
			this.Pt2 += delta;
#endif
		}
	}
}
