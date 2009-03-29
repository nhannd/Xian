#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
		private RectangleF _rectangle = new RectangleF(0, 0, 0, 0);
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
					return _rectangle.Location;
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					return base.SpatialTransform.ConvertToDestination(_rectangle.Location);
				}
			}
			set
			{
				if (FloatComparer.AreEqual(this.TopLeft, value))
					return;

				if (base.CoordinateSystem == CoordinateSystem.Source)
				{
					_rectangle.Width = _rectangle.Right - value.X;
					_rectangle.Height = _rectangle.Bottom - value.Y;
					_rectangle.Location = value;
				}
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");

					_rectangle.Width = _rectangle.Right - base.SpatialTransform.ConvertToSource(value).X;
					_rectangle.Height = _rectangle.Bottom - base.SpatialTransform.ConvertToSource(value).Y;
					_rectangle.Location = base.SpatialTransform.ConvertToSource(value);
				}

				EventsHelper.Fire(_topLeftChangedEvent, this, new PointChangedEventArgs(this.TopLeft));
				base.NotifyPropertyChanged("TopLeft");
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
				PointF bottomRight = new PointF(_rectangle.Right, _rectangle.Bottom);

				if (base.CoordinateSystem == CoordinateSystem.Source)
					return bottomRight;
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					return base.SpatialTransform.ConvertToDestination(bottomRight);
				}
			}
			set
			{
				if (FloatComparer.AreEqual(this.BottomRight, value))
					return;

				if (base.CoordinateSystem == CoordinateSystem.Source)
				{
					_rectangle.Width = value.X - _rectangle.X;
					_rectangle.Height = value.Y - _rectangle.Y;
				}
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					PointF pointSrc = base.SpatialTransform.ConvertToSource(value);

					_rectangle.Width = pointSrc.X - _rectangle.X;
					_rectangle.Height = pointSrc.Y - _rectangle.Y;
				}

				EventsHelper.Fire(_bottomRightChangedEvent, this, new PointChangedEventArgs(this.BottomRight));
				base.NotifyPropertyChanged("BottomRight");
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
				{
					return _rectangle.Width;
				}
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");

					PointF topLeft = base.SpatialTransform.ConvertToDestination(_rectangle.Location);
					PointF bottomRight = base.SpatialTransform.ConvertToDestination(new PointF(_rectangle.Right, _rectangle.Bottom));

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
				{
					return _rectangle.Height;
				}
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");

					PointF topLeft = base.SpatialTransform.ConvertToDestination(_rectangle.Location);
					PointF bottomRight = base.SpatialTransform.ConvertToDestination(new PointF(_rectangle.Right, _rectangle.Bottom));

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
				{
					return _rectangle;
				}
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");

					return base.SpatialTransform.ConvertToDestination(_rectangle);
				}
			}
		}

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