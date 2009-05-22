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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A linear <see cref="InvariantPrimitive"/>.
	/// </summary>
	/// <remarks>
	/// <para>This primitive graphic defines a line whose position can be fixed to the
	/// source coordinate system and whose length will be fixed relative to the
	/// destination coordinate system.</para>
	/// <para>The <see cref="InvariantLinePrimitive.AnchorPoint"/> defines the point
	/// that is affixed to the source coordinate system, and the <see cref="InvariantBoundablePrimitive.InvariantTopLeft"/>
	/// and <see cref="InvariantBoundablePrimitive.InvariantBottomRight"/> properties define the length
	/// and orientation of the line.</para>
	/// </remarks>
	[Cloneable(true)]
	public class InvariantLinePrimitive : InvariantBoundablePrimitive, ILineSegmentGraphic
	{
		/// <summary>
		/// Constructs a new invariant line primitive.
		/// </summary>
		public InvariantLinePrimitive() {}

		/// <summary>
		/// Performs a hit test on the <see cref="Graphic"/> at a given point.
		/// </summary>
		/// <param name="point">The mouse position in destination coordinates.</param>
		/// <returns>
		/// <b>True</b> if <paramref name="point"/> "hits" the <see cref="InvariantLinePrimitive"/>,
		/// <b>false</b> otherwise.
		/// </returns>
		public override bool HitTest(Point point)
		{
			this.CoordinateSystem = CoordinateSystem.Destination;
			try
			{
				PointF output = new PointF();
				double distance = Vector.DistanceFromPointToLine(point, this.TopLeft, this.BottomRight, ref output);
				return distance < HitTestDistance;
			}
			finally
			{
				this.ResetCoordinateSystem();
			}
		}

		public override PointF GetClosestPoint(PointF point)
		{
			PointF result = PointF.Empty;
			RectangleF rect = this.Rectangle;
			Vector.DistanceFromPointToLine(point, new PointF(rect.Left, rect.Top), new PointF(rect.Right, rect.Bottom), ref result);
			return result;
		}

		public override bool Contains(PointF point)
		{
			return false;
		}

		/// <summary>
		/// The endpoint of the line as specified by <see cref="InvariantBoundablePrimitive.TopLeft"/> in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		PointF ILineSegmentGraphic.Pt1
		{
			get { return this.TopLeft; }
			set { throw new NotSupportedException(); }
		}

		/// <summary>
		/// The endpoint of the line as specified by <see cref="InvariantBoundablePrimitive.BottomRight"/> in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		PointF ILineSegmentGraphic.Pt2
		{
			get { return this.BottomRight; }
			set { throw new NotSupportedException(); }
		}

		event EventHandler<PointChangedEventArgs> ILineSegmentGraphic.Pt1Changed
		{
			add { }
			remove { }
		}

		event EventHandler<PointChangedEventArgs> ILineSegmentGraphic.Pt2Changed
		{
			add { }
			remove { }
		}
	}
}