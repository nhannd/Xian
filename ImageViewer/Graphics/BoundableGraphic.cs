#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A <see cref="VectorGraphic"/> that can be described by a
	/// rectangular bounding box.
	/// </summary>
	/// <remarks>
	/// Rectangles and ellipses are examples of graphics that can be
	/// described by a rectangular bounding box.
	/// </remarks>
	[Cloneable(true)]
	public abstract class BoundableGraphic : VectorGraphic, IBoundableGraphic
	{
		private PointF _topLeft = new PointF(0, 0);
		private PointF _bottomRight = new PointF(0, 0);
		private event EventHandler<PointChangedEventArgs> _topLeftChangedEvent;
		private event EventHandler<PointChangedEventArgs> _bottomRightChangedEvent;

		/// <summary>
		/// Constructor.
		/// </summary>
		protected BoundableGraphic()
		{
		}

		/// <summary>
		/// Gets or sets the top-left corner of the rectangle
		/// in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		public PointF TopLeft
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
					return _topLeft;
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					return base.SpatialTransform.ConvertToDestination(_topLeft);
				}
			}
			set
			{
				if (FloatComparer.AreEqual(this.TopLeft, value))
					return;

				if (base.CoordinateSystem == CoordinateSystem.Source)
					_topLeft = value;
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					_topLeft = base.SpatialTransform.ConvertToSource(value);
				}

				EventsHelper.Fire(_topLeftChangedEvent, this, new PointChangedEventArgs(this.TopLeft));
				base.NotifyVisualStateChanged("TopLeft");
			}
		}

		/// <summary>
		/// Gets or sets the bottom-right corner of the rectangle
		/// in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		public PointF BottomRight
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
					return _bottomRight;
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					return base.SpatialTransform.ConvertToDestination(_bottomRight);
				}
			}
			set
			{
				if (FloatComparer.AreEqual(this.BottomRight, value))
					return;

				if (base.CoordinateSystem == CoordinateSystem.Source)
					_bottomRight = value;
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					_bottomRight = base.SpatialTransform.ConvertToSource(value);
				}

				EventsHelper.Fire(_bottomRightChangedEvent, this, new PointChangedEventArgs(this.BottomRight));
				base.NotifyVisualStateChanged("BottomRight");
			}
		}

		/// <summary>
		/// Gets the width of the rectangle in either source or destination pixels.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination pixels.
		/// </remarks>
		public float Width
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
					return _bottomRight.X - _topLeft.X;
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					PointF topLeft = base.SpatialTransform.ConvertToDestination(_topLeft);
					PointF bottomRight = base.SpatialTransform.ConvertToDestination(_bottomRight);

					return bottomRight.X - topLeft.X;
				}
			}
		}

		/// <summary>
		/// Gets the height of the rectangle in either source or destination pixels.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination pixels.
		/// </remarks>
		public float Height
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
					return _bottomRight.Y - _topLeft.Y;
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					PointF topLeft = base.SpatialTransform.ConvertToDestination(_topLeft);
					PointF bottomRight = base.SpatialTransform.ConvertToDestination(_bottomRight);

					return bottomRight.Y - topLeft.Y;
				}
			}
		}

		/// <summary>
		/// Gets the rectangle that defines a <see cref="BoundableGraphic"/>.
		/// </summary>
		public RectangleF Rectangle
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
					return RectangleF.FromLTRB(_topLeft.X, _topLeft.Y, _bottomRight.X, _bottomRight.Y);
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					PointF topLeft = base.SpatialTransform.ConvertToDestination(_topLeft);
					PointF bottomRight = base.SpatialTransform.ConvertToDestination(_bottomRight);

					return RectangleF.FromLTRB(topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);
				}
			}
		}

		/// <summary>
		/// Gets the tightest bounding box that encloses the graphic in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		public override RectangleF BoundingBox
		{
			get { return RectangleUtilities.ConvertToPositiveRectangle(this.Rectangle); }
		}

		/// <summary>
		/// Occurs when the <see cref="TopLeft"/> property changed.
		/// </summary>
		public event EventHandler<PointChangedEventArgs> TopLeftChanged
		{
			add { _topLeftChangedEvent += value; }
			remove { _topLeftChangedEvent -= value; }
		}

		/// <summary>
		/// Occurs when the <see cref="BottomRight"/> property changed.
		/// </summary>
		public event EventHandler<PointChangedEventArgs> BottomRightChanged
		{
			add { _bottomRightChangedEvent += value; }
			remove { _bottomRightChangedEvent -= value; }
		}

		/// <summary>
		/// Moves the <see cref="RectanglePrimitive"/> by a specified delta.
		/// </summary>
		/// <param name="delta">The distance to move.</param>
		/// <remarks>
		/// Depending on the value of <see cref="Graphic.CoordinateSystem"/>,
		/// <paramref name="delta"/> will be interpreted in either source
		/// or destination coordinates.
		/// </remarks>
		public override void Move(SizeF delta)
		{
			this.TopLeft += delta;
			this.BottomRight += delta;
		}

		/// <summary>
		/// Returns a value indicating whether the specified point is
		/// contained in the graphic.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public abstract bool Contains(PointF point);
	}
}