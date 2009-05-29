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
using Matrix = System.Drawing.Drawing2D.Matrix;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// An <see cref="InvariantPrimitive"/> that can be described by a
	/// rectangular bounding box.
	/// </summary>
	[Cloneable(true)]
	public abstract class InvariantBoundablePrimitive : InvariantPrimitive, IBoundableGraphic
	{
		private event EventHandler<PointChangedEventArgs> _topLeftChanged;
		private event EventHandler<PointChangedEventArgs> _bottomRightChanged;

		private PointF _topLeft = new PointF(0,0);
		private PointF _bottomRight = new PointF(0,0);

		/// <summary>
		/// Constructor.
		/// </summary>
		protected InvariantBoundablePrimitive()
		{
		}

		/// <summary>
		/// Gets the top left corner of the rectangle in either source or destination coordinates.
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
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");

					this.CoordinateSystem = CoordinateSystem.Destination;
					PointF topLeft = this.SpatialTransform.ConvertToSource(this.TopLeft);
					this.ResetCoordinateSystem();

					return topLeft;
				}
				else
				{
					return ConvertInvariantToDestination(InvariantTopLeft);
				}
			}
		}

		/// <summary>
		/// Gets or sets the top left corner of the rectangle in <i>screen</i> pixels relative to 
		/// the anchor point.
		/// </summary>
		/// <remarks>
		/// <see cref="InvariantTopLeft"/> is different from <see cref="TopLeft"/> in that
		/// it is the top left corner of the rectangle in screen pixels where (0,0)
		/// is the anchor point. For example, if you wanted an
		/// invariant square of size 9x9 screen pixels and the anchor point is in the middle
		/// of the square, <see cref="InvariantTopLeft"/> would be (-4,-4) and 
		/// <see cref="InvariantBottomRight"/> would be (4,4).
		/// </remarks>
		public PointF InvariantTopLeft
		{
			get { return _topLeft; }
			set
			{
				if (_topLeft != value)
				{
					_topLeft = value;
					this.OnTopLeftChanged();
				}
			}
		}

		/// <summary>
		/// Gets the bottom right corner of the rectangle in either source or destination coordinates.
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
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");

					this.CoordinateSystem = CoordinateSystem.Destination;
					PointF bottomRight = this.SpatialTransform.ConvertToSource(this.BottomRight);
					this.ResetCoordinateSystem();

					return bottomRight;
				}
				else
				{
					return ConvertInvariantToDestination(InvariantBottomRight);
				}
			}
		}

		/// <summary>
		/// Gets or sets the bottom right corner of the rectangle in <i>screen</i> pixels relative to 
		/// the anchor point.
		/// </summary>
		/// <remarks>
		/// <see cref="InvariantBottomRight"/> is different from <see cref="BottomRight"/> in that
		/// it is the bottom right corner of the rectangle in screen pixels where (0,0)
		/// is the anchor point. For example, if you wanted an
		/// invariant square of size 9x9 screen pixels and the anchor point is in the middle
		/// of the square, <see cref="InvariantTopLeft"/> would be (-4,-4) and 
		/// <see cref="InvariantBottomRight"/> would be (4,4).
		/// </remarks>
		public PointF InvariantBottomRight
		{
			get { return _bottomRight; }
			set
			{
				if (_bottomRight != value)
				{
					_bottomRight = value;
					this.OnBottomRightChanged();
				}
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
				return this.BottomRight.X - this.TopLeft.X;
			}
		}

		/// <summary>
		/// Gets the height of the rectangle in either source or destination pixels.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		public float Height
		{
			get
			{
				return this.BottomRight.Y - this.TopLeft.Y;
			}
		}

		/// <summary>
		/// Occurs when the value of <see cref="TopLeft"/> changes.
		/// </summary>
		public event EventHandler<PointChangedEventArgs> TopLeftChanged
		{
			add { _topLeftChanged += value; }
			remove { _topLeftChanged -= value; }
		}

		/// <summary>
		/// Occurs when the value of <see cref="BottomRight"/> changes.
		/// </summary>
		public event EventHandler<PointChangedEventArgs> BottomRightChanged
		{
			add { _bottomRightChanged += value; }
			remove { _bottomRightChanged -= value; }
		}

		/// <summary>
		/// Returns a value indicating whether the specified point is
		/// contained in the graphic.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public abstract bool Contains(PointF point);

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
		/// Gets the rectangle that defines a <see cref="BoundableGraphic"/>.
		/// </summary>
		public RectangleF Rectangle
		{
			get
			{
				return new RectangleF(this.TopLeft.X, this.TopLeft.Y, this.Width, this.Height);
			}
		}

		/// <summary>
		/// Called when the value of <see cref="InvariantPrimitive.Location"/> changes.
		/// </summary>
		protected override void OnLocationChanged()
		{
			this.OnTopLeftChanged();
			this.OnBottomRightChanged();
			base.OnLocationChanged();
		}

		private PointF ConvertInvariantToDestination(PointF invariantPoint)
		{
			PointF xVector = new PointF(100, 0);
			SizeF xVectorTransformed = base.SpatialTransform.ConvertToDestination(new SizeF(xVector));

			//figure out where the source x-axis went in destination
			int rotation = (int)Math.Round(Vector.SubtendedAngle(xVectorTransformed.ToPointF(), PointF.Empty, xVector));
			if (rotation < 0)
				rotation += 360;

			Matrix m = new Matrix();
			m.Rotate(rotation);
			PointF[] pt = { invariantPoint };
			m.TransformPoints(pt);
			m.Dispose();

			return new PointF(base.Location.X + pt[0].X, base.Location.Y + pt[0].Y);
		}

		/// <summary>
		/// Called when the value of <see cref="TopLeft"/> changes.
		/// </summary>
		protected virtual void OnTopLeftChanged()
		{
			EventsHelper.Fire(_topLeftChanged, this, new PointChangedEventArgs(this.TopLeft));
		}

		/// <summary>
		/// Called when the value of <see cref="BottomRight"/> changes.
		/// </summary>
		protected virtual void OnBottomRightChanged()
		{
			EventsHelper.Fire(_bottomRightChanged, this, new PointChangedEventArgs(this.BottomRight));
		}

		#region IBoundableGraphic Members

		PointF IBoundableGraphic.TopLeft
		{
			get { return this.TopLeft; }
			set { throw new NotSupportedException(); }
		}

		PointF IBoundableGraphic.BottomRight
		{
			get { return this.BottomRight; }
			set { throw new NotSupportedException(); }
		}

		#endregion
	}
}