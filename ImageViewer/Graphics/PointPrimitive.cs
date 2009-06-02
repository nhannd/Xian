#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
