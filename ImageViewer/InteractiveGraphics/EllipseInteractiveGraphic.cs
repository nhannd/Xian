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
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.PresentationStates;
using ClearCanvas.ImageViewer.RoiGraphics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// A interactive elliptical graphic.
	/// </summary>
	[Cloneable]
	[DicomSerializableGraphicAnnotation(typeof(EllipseInteractiveGraphicAnnotationSerializer))]
	public class EllipseInteractiveGraphic : BoundableInteractiveGraphic
	{
		/// <summary>
		/// Initializes a new instance of <see cref="RectangleInteractiveGraphic"/>.
		/// </summary>
		public EllipseInteractiveGraphic()
		{
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected EllipseInteractiveGraphic(EllipseInteractiveGraphic source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		/// <summary>
		/// Creates an <see cref="EllipsePrimitive"/>.
		/// </summary>
		/// <returns></returns>
		protected override BoundableGraphic CreateBoundableGraphic()
		{
			return new EllipsePrimitive();
		}

		/// <summary>
		/// Peforms a hit test on the <see cref="EllipseInteractiveGraphic"/>
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		/// <remarks>
		/// The active region of this hit test is the ellipse itself, as well as
		/// the bounding rectangle.
		/// </remarks>
		public override bool HitTest(Point point)
		{
			this.CoordinateSystem = CoordinateSystem.Source;
			RectangleF boundingBox = new RectangleF(this.Left, this.Top, this.Width, this.Height);

			bool result = RectanglePrimitive.HitTest(this.SpatialTransform.ConvertToSource(point),
			                           boundingBox, this.SpatialTransform);

			this.ResetCoordinateSystem();

			return result || base.HitTest(point);
		}

		/// <summary>
		/// Gets the point where the ellipse intersects the line whose end points
		/// are the center of the ellipse and the specified point.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public override PointF GetClosestPoint(PointF point)
		{
			// Semi major/minor axes
			float a = this.Width/2;
			float b = this.Height/2;

			// Center of ellipse
			float x1 = this.Left + a;
			float y1 = this.Top + b;

			PointF center = new PointF(x1,y1);

			return IntersectEllipseAndLine(a, b, center, point);
		}

		public override Roi CreateRoiInformation()
		{
			return new EllipticalRoi(this);
		}

		/// <summary>
		/// Finds the intersection between an ellipse and a line that starts at the
		/// center of the ellipse and ends at an aribtrary point.
		/// </summary>
		internal static PointF IntersectEllipseAndLine(float a, float b, PointF center, PointF point)
		{
			/*
			 * The point of intersection (P) between the center of the ellipse and the test point (Pt)
			 * where the center of the ellipse is at (0, 0) can be described by the vector equation:
			 * _     __ 
			 * P = m*Pt
			 * 
			 * which yields two equations:
			 * 
			 * x = m * xt (1)
			 * y = m * yt (2)
			 * 
			 * An ellipse centered at (0, 0) is described by the equation:
			 * 
			 * x^2/a^2 + y^2/b^2 = 1 (3)
			 * 
			 * substituting (1) and (2) into (3) gives:
			 * 
			 * m^2*xt^2/a^2 + m^2*yt^2/b^2 = 1
			 * m^2*(xt^2*b^2 + yt^2*a^2) = a*b
			 * 
			 * finally,
			 * 
			 * m = a*b/Sqrt(xt^2*b^2 + yt^2*a^2) (where a^2*yt^2 > 0 and/or b^2*yt^2 > 0)
			 * 
			 * which is a constant for a given ellipse.
			 * 
			 * The intersection point (x, y) can then be found by substituting m into (1) and (2).
			*/

			PointF testPoint = new PointF(point.X - center.X, point.Y - center.Y);

			float denominator = (float)Math.Sqrt(	testPoint.X * testPoint.X * b * b +
													testPoint.Y * testPoint.Y * a * a);

			if (FloatComparer.AreEqual(denominator, 0.0F, 0.001F))
				return center;

			float m = Math.Abs(a*b/denominator);

			return new PointF(center.X + m*testPoint.X, center.Y + m*testPoint.Y);
		}
	}
}
