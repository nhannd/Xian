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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.PresentationStates;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// A interactive rectangular graphic.
	/// </summary>
	[Cloneable]
	[DicomSerializableGraphicAnnotation(typeof(RectangleInteractiveGraphicAnnotationSerializer))]
	public class RectangleInteractiveGraphic : BoundableInteractiveGraphic
	{
		/// <summary>
		/// Initializes a new instance of <see cref="RectangleInteractiveGraphic"/>.
		/// </summary>
		public RectangleInteractiveGraphic()
		{
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected RectangleInteractiveGraphic(RectangleInteractiveGraphic source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		/// <summary>
		/// Creates a <see cref="RectanglePrimitive"/>
		/// </summary>
		/// <returns></returns>
		protected override BoundableGraphic CreateBoundableGraphic()
		{
			return new RectanglePrimitive();
		}

		/// <summary>
		/// Gets the point on the <see cref="RectangleInteractiveGraphic"/>
		/// closest to the specified point.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public override PointF GetClosestPoint(PointF point)
		{
			double currentDistance;
			double shortestDistance = double.MaxValue;
			PointF currentPoint = new PointF(0, 0);
			PointF closestPoint = new PointF(0, 0);

			PointF ptTopLeft = this.TopLeft;
			PointF ptBottomRight = this.BottomRight;
			PointF ptTopRight = new PointF(ptBottomRight.X, ptTopLeft.Y);
			PointF ptBottomLeft = new PointF(ptTopLeft.X, ptBottomRight.Y);

			currentDistance = Vector.DistanceFromPointToLine(point, ptTopLeft, ptTopRight, ref currentPoint);

			if (currentDistance < shortestDistance)
			{
				shortestDistance = currentDistance;
				closestPoint = currentPoint;
			}

			currentDistance = Vector.DistanceFromPointToLine(point, ptTopRight, ptBottomRight, ref currentPoint);

			if (currentDistance < shortestDistance)
			{
				shortestDistance = currentDistance;
				closestPoint = currentPoint;
			}

			currentDistance = Vector.DistanceFromPointToLine(point, ptBottomRight, ptBottomLeft, ref currentPoint);

			if (currentDistance < shortestDistance)
			{
				shortestDistance = currentDistance;
				closestPoint = currentPoint;
			}

			currentDistance = Vector.DistanceFromPointToLine(point, ptBottomLeft, ptTopLeft, ref currentPoint);

			if (currentDistance < shortestDistance)
			{
				closestPoint = currentPoint;
			}

			return closestPoint;
		}
	}
}
