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
	/// A primitive point graphic.
	/// </summary>
	[Cloneable(true)]
	public class PointPrimitive : VectorGraphic, IPointGraphic
	{
		private event EventHandler _pointChanged;
		private PointF _point;

		/// <summary>
		/// Initializes a new instance of <see cref="PointPrimitive"/>.
		/// </summary>
		public PointPrimitive()
		{
		}

		/// <summary>
		/// Gets or sets the location of the point in either source
		/// or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		public PointF Point
		{
			get 
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
					return _point;
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					return base.SpatialTransform.ConvertToDestination(_point);
				}
			}
			set 
			{
				if (FloatComparer.AreEqual(this.Point, value))
					return;

				if (base.CoordinateSystem == CoordinateSystem.Source)
					_point = value;
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					_point = base.SpatialTransform.ConvertToSource(value);
				}

				this.NotifyPointChanged();
				base.NotifyVisualStateChanged("Point");
			}
		}

		/// <summary>
		/// Occurs when the value of <see cref="IPointGraphic.Point"/> changes.
		/// </summary>
		public event EventHandler PointChanged
		{
			add { _pointChanged += value; }
			remove { _pointChanged -= value; }
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
			get { return new RectangleF(this.Point, SizeF.Empty); }
		}

		/// <summary>
		/// Performs a hit test on the <see cref="PointPrimitive"/>.
		/// </summary>
		/// <param name="point">The test point in destination coordinates.</param>
		/// <returns></returns>
		public override bool HitTest(Point point)
		{
			return FloatComparer.AreEqual(base.SpatialTransform.ConvertToDestination(_point), point);
		}

		/// <summary>
		/// Gets the point on the <see cref="Graphic"/> closest to the specified point.
		/// </summary>
		/// <param name="point">A point in either source or destination coordinates.</param>
		/// <returns>The point on the graphic closest to the given <paramref name="point"/>.</returns>
		/// <remarks>
		/// <para>
		/// Depending on the value of <see cref="Graphic.CoordinateSystem"/>,
		/// the computation will be carried out in either source
		/// or destination coordinates.</para>
		/// </remarks>
		public override PointF GetClosestPoint(PointF point)
		{
			return this.Point;
		}

		/// <summary>
		/// Move the <see cref="PointPrimitive"/> by a specified delta.
		/// </summary>
		/// <param name="delta"></param>
		public override void Move(SizeF delta)
		{
			this.Point += delta;
		}

		private void NotifyPointChanged()
		{
			EventsHelper.Fire(_pointChanged, this, EventArgs.Empty);
		}
	}
}
