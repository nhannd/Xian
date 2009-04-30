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

using System.Drawing;
using System.Drawing.Drawing2D;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.PresentationStates;
using ClearCanvas.ImageViewer.PresentationStates.GraphicAnnotationSerializers;
using ClearCanvas.ImageViewer.RoiGraphics;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A primitive rectangle graphic.
	/// </summary>
	[Cloneable(true)]
	[DicomSerializableGraphicAnnotation(typeof (RectangleGraphicAnnotationSerializer))]
	public class RectanglePrimitive : BoundableGraphic
	{
		/// <summary>
		/// Initializes a new instance of <see cref="RectanglePrimitive"/>.
		/// </summary>
		public RectanglePrimitive() 
		{
		}

		/// <summary>
		/// Performs a hit test on the <see cref="RectanglePrimitive"/> at a given point.
		/// </summary>
		/// <param name="point">The mouse position in destination coordinates.</param>
		/// <returns>
		/// <b>True</b> if <paramref name="point"/> "hits" the <see cref="RectanglePrimitive"/>,
		/// <b>false</b> otherwise.
		/// </returns>
		/// <remarks>
		/// A "hit" is defined as when the mouse position is <see cref="VectorGraphic.HitTestDistance"/>
		/// screen pixels away from any point on the rectangle.
		/// </remarks>
		public override bool HitTest(Point point)
		{
			this.CoordinateSystem = CoordinateSystem.Source;
			bool result = HitTest(this.SpatialTransform.ConvertToSource(point), this.Rectangle, this.SpatialTransform);
			this.ResetCoordinateSystem();

			return result;
		}

		/// <summary>
		/// Gets the point on the <see cref="RectanglePrimitive"/> closest to the specified point.
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
			return GetClosestPoint(point, this.BoundingBox);
		}

		/// <summary>
		/// Returns a value indicating whether the specified point is contained
		/// in the rectangle.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public override bool Contains(PointF point)
		{
			return this.Rectangle.Contains(point);
		}

		public override Roi CreateRoiInformation()
		{
			return new RectangularRoi(this);
		}

		internal static bool HitTest(PointF point, RectangleF rectangle, SpatialTransform transform)
		{
			GraphicsPath path = new GraphicsPath();
			path.AddRectangle(RectangleUtilities.ConvertToPositiveRectangle(rectangle));

			Pen pen = new Pen(Brushes.White, HitTestDistance/transform.CumulativeScale);
			bool result = path.IsOutlineVisible(point, pen);

			path.Dispose();
			pen.Dispose();

			return result;
		}

		internal static PointF GetClosestPoint(PointF point, RectangleF rectangle)
		{
			double currentDistance;
			double shortestDistance = double.MaxValue;
			PointF currentPoint = new PointF(0, 0);
			PointF closestPoint = new PointF(0, 0);

			PointF ptTopLeft = new PointF(rectangle.Left, rectangle.Top);
			PointF ptBottomRight = new PointF(rectangle.Right, rectangle.Bottom);
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
