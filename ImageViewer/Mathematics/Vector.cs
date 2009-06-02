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

namespace ClearCanvas.ImageViewer.Mathematics
{
	/// <summary>
	/// A collection vector related methods.
	/// </summary>
	public static class Vector
	{
		/// <summary>
		/// Calculates the angle subtended by two line segments that meet at a vertex.
		/// </summary>
		public static double SubtendedAngle(PointF start, PointF vertex, PointF end)
		{
			Vector3D vertexPositionVector = new Vector3D(vertex.X, vertex.Y, 0);
			Vector3D a = new Vector3D(start.X, start.Y, 0) - vertexPositionVector;
			Vector3D b = new Vector3D(end.X, end.Y, 0) - vertexPositionVector;

			float dotProduct = a.Dot(b);

			Vector3D crossProduct = a.Cross(b);

			float magA = a.Magnitude;
			float magB = b.Magnitude;

			if (FloatComparer.AreEqual(magA, 0F) || FloatComparer.AreEqual(magB, 0F))
				return 0;

			double cosTheta = dotProduct / magA / magB;

			// Make sure cosTheta is within bounds so we don't
			// get any errors when we take the acos.
			if (cosTheta > 1.0f)
				cosTheta = 1.0f;

			if (cosTheta < -1.0f)
				cosTheta = -1.0f;

			double theta = Math.Acos(cosTheta) * (crossProduct.Z == 0 ? 1 : -Math.Sign(crossProduct.Z));
			double thetaInDegrees = theta / Math.PI * 180;

			return thetaInDegrees;
		}

		/// <summary>
		/// Calculates the distance between two points.
		/// </summary>
		/// <param name="pt1"></param>
		/// <param name="pt2"></param>
		/// <returns></returns>
		public static double Distance(Point pt1, Point pt2)
		{
			int deltaX = pt2.X - pt1.X;
			int deltaY = pt2.Y - pt1.Y;

			return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
		}

		/// <summary>
		/// Calculates the distance between two points.
		/// </summary>
		/// <param name="pt1"></param>
		/// <param name="pt2"></param>
		/// <returns></returns>
		public static double Distance(PointF pt1, PointF pt2)
		{
			float deltaX = pt2.X - pt1.X;
			float deltaY = pt2.Y - pt1.Y;

			return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
		}

		/// <summary>
		/// Finds the midpoint between two points.
		/// </summary>
		/// <param name="pt1"></param>
		/// <param name="pt2"></param>
		/// <returns></returns>
		public static PointF Midpoint(PointF pt1, PointF pt2)
		{
			float x = (pt1.X + pt2.X) / 2;
			float y = (pt1.Y + pt2.Y) / 2;

			return new PointF(x, y);
		}

		/// <summary>
		/// Finds the midpoint between two points.
		/// </summary>
		/// <param name="pt1"></param>
		/// <param name="pt2"></param>
		/// <returns></returns>
		public static Point Midpoint(Point pt1, Point pt2)
		{
			int x = (pt1.X + pt2.X) / 2;
			int y = (pt1.Y + pt2.Y) / 2;

			return new Point(x, y);
		}

		/// <summary>
		/// Computes the unit vector of the vector from the origin towards the specified <paramref name="vector">point</paramref>.
		/// </summary>
		public static SizeF UnitVector(PointF vector)
		{
			return UnitVector(PointF.Empty, vector);
		}

		/// <summary>
		/// Computes the unit vector of the vector from <paramref name="origin"/> towards the specified <paramref name="vector">point</paramref>.
		/// </summary>
		public static SizeF UnitVector(PointF origin, PointF vector)
		{
			if (FloatComparer.AreEqual(origin, vector))
				throw new ArgumentException("The arguments must define a valid vector.");
			float deltaX = vector.X - origin.X;
			float deltaY = vector.Y - origin.Y;
			double magnitude = Math.Sqrt(deltaX*deltaX + deltaY*deltaY);
			return new SizeF((float) (deltaX/magnitude), (float) (deltaY/magnitude));
		}

		/// <summary>
		/// Calculates the shortest distance from a point to a line segment.
		/// </summary>
		/// <param name="ptTest">Point to be tested.</param>
		/// <param name="pt1">Beginning of line segment.</param>
		/// <param name="pt2">End of line segement.</param>
		/// <param name="ptNearest">The point on line segment that is
		/// closest to the test point.</param>
		/// <returns>The distance from the test point to
		/// the nearest point on the line segment.</returns>
		static public double DistanceFromPointToLine(PointF ptTest, PointF pt1, PointF pt2, ref PointF ptNearest)
		{
			float distanceX;
			float distanceY;
			double distance;

			// Point on line segment nearest to pt0
			float dx = pt2.X - pt1.X;
			float dy = pt2.Y - pt1.Y;

			// It's a point, not a line
			if (dx == 0 && dy == 0)
			{
				ptNearest.X = pt1.X;
				ptNearest.Y = pt1.Y;
			}
			else
			{
				// Parameter
				double t = ((ptTest.X - pt1.X) * dx + (ptTest.Y - pt1.Y) * dy) / (double)(dx * dx + dy * dy);

				// Nearest point is pt1
				if (t < 0)
				{
					ptNearest = pt1;
				}
				// Nearest point is pt2
				else if (t > 1)
				{
					ptNearest = pt2;
				}
				// Nearest point is on the line segment
				else
				{
					// Parametric equation
					ptNearest.X = (int)(pt1.X + t * dx);
					ptNearest.Y = (int)(pt1.Y + t * dy);
				}
			}

			distanceX = ptTest.X - ptNearest.X;
			distanceY = ptTest.Y - ptNearest.Y;
			distance = Math.Sqrt(distanceX * distanceX + distanceY * distanceY);

			return distance;
		}

		/// <summary>
		/// Possible arrangements of two line segments.
		/// </summary>
		public enum LineSegments
		{
			/// <summary>
			/// The line segments do not interesect.
			/// </summary>
			DoNotIntersect,
			/// <summary>
			/// The line segments intersect.
			/// </summary>
			Intersect,
			/// <summary>
			/// The line segments are colinear.
			/// </summary>
			Colinear
		}

		/// <summary>
		/// Determines whether two line segments intersect, do not intersect
		/// or are colinear.
		/// </summary>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <param name="q1"></param>
		/// <param name="q2"></param>
		/// <param name="intersectionPoint"></param>
		/// <returns></returns>
		public static LineSegments LineSegmentIntersection(
			Point p1,
			Point p2,
			Point q1,
			Point q2,
			out Point intersectionPoint)
		{
			int x1 = p1.X;
			int y1 = p1.Y;
			int x2 = p2.X;
			int y2 = p2.Y;
			int x3 = q1.X;
			int y3 = q1.Y;
			int x4 = q2.X;
			int y4 = q2.Y;

			int a1, a2, b1, b2, c1, c2; /* Coefficients of line eqns. */
			int r1, r2, r3, r4;         /* 'Sign' values */
			int denom, offset, num;     /* Intermediate values */

			/* Compute a1, b1, c1, where line joining points 1 and 2
			 * is "a1 x  +  b1 y  +  c1  =  0".
			 */

			a1 = y2 - y1;
			b1 = x1 - x2;
			c1 = x2 * y1 - x1 * y2;

			/* Compute r3 and r4.
			 */


			r3 = a1 * x3 + b1 * y3 + c1;
			r4 = a1 * x4 + b1 * y4 + c1;

			/* Check signs of r3 and r4.  If both point 3 and point 4 lie on
			 * same side of line 1, the line segments do not intersect.
			 */

			if (r3 != 0 &&
				r4 != 0 &&
				Math.Sign(r3) == Math.Sign(r4))
			{
				intersectionPoint = new Point(0, 0);
				return LineSegments.DoNotIntersect;
			}

			/* Compute a2, b2, c2 */

			a2 = y4 - y3;
			b2 = x3 - x4;
			c2 = x4 * y3 - x3 * y4;

			/* Compute r1 and r2 */

			r1 = a2 * x1 + b2 * y1 + c2;
			r2 = a2 * x2 + b2 * y2 + c2;

			/* Check signs of r1 and r2.  If both point 1 and point 2 lie
			 * on same side of second line segment, the line segments do
			 * not intersect.
			 */

			if (r1 != 0 &&
				r2 != 0 &&
				Math.Sign(r1) == Math.Sign(r2))
			{
				intersectionPoint = new Point(0, 0);
				return LineSegments.DoNotIntersect;
			}

			/* Line segments intersect: compute intersection point. 
			 */

			denom = a1 * b2 - a2 * b1;
			if (denom == 0)
			{
				intersectionPoint = new Point(0, 0);
				return LineSegments.Colinear;
			}

			offset = denom < 0 ? -denom / 2 : denom / 2;

			/* The denom/2 is to get rounding instead of truncating.  It
			 * is added or subtracted to the numerator, depending upon the
			 * sign of the numerator.
			 */

			num = b1 * c2 - b2 * c1;
			int x = (num < 0 ? num - offset : num + offset) / denom;

			num = a2 * c1 - a1 * c2;
			int y = (num < 0 ? num - offset : num + offset) / denom;

			intersectionPoint = new Point(x, y);

			return LineSegments.Intersect;
		}

		/// <summary>
		/// Determines whether two line segments intersect, do not intersect
		/// or are colinear.
		/// </summary>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <param name="q1"></param>
		/// <param name="q2"></param>
		/// <param name="intersectionPoint"></param>
		/// <returns></returns>
		public static LineSegments LineSegmentIntersection(
			PointF p1,
			PointF p2,
			PointF q1,
			PointF q2,
			out PointF intersectionPoint)
		{
			float x1 = p1.X;
			float y1 = p1.Y;
			float x2 = p2.X;
			float y2 = p2.Y;
			float x3 = q1.X;
			float y3 = q1.Y;
			float x4 = q2.X;
			float y4 = q2.Y;

			float a1, a2, b1, b2, c1, c2; /* Coefficients of line eqns. */
			float r1, r2, r3, r4;         /* 'Sign' values */
			float denom, offset, num;     /* Intermediate values */

			/* Compute a1, b1, c1, where line joining points 1 and 2
			 * is "a1 x  +  b1 y  +  c1  =  0".
			 */

			a1 = y2 - y1;
			b1 = x1 - x2;
			c1 = x2 * y1 - x1 * y2;

			/* Compute r3 and r4.
			 */


			r3 = a1 * x3 + b1 * y3 + c1;
			r4 = a1 * x4 + b1 * y4 + c1;

			/* Check signs of r3 and r4.  If both point 3 and point 4 lie on
			 * same side of line 1, the line segments do not intersect.
			 */

			if (r3 != 0 &&
				r4 != 0 &&
				Math.Sign(r3) == Math.Sign(r4))
			{
				intersectionPoint = new PointF(0,0);
				return LineSegments.DoNotIntersect;
			}

			/* Compute a2, b2, c2 */

			a2 = y4 - y3;
			b2 = x3 - x4;
			c2 = x4 * y3 - x3 * y4;

			/* Compute r1 and r2 */

			r1 = a2 * x1 + b2 * y1 + c2;
			r2 = a2 * x2 + b2 * y2 + c2;

			/* Check signs of r1 and r2.  If both point 1 and point 2 lie
			 * on same side of second line segment, the line segments do
			 * not intersect.
			 */

			if (r1 != 0 &&
				r2 != 0 &&
				Math.Sign(r1) == Math.Sign(r2))
			{
				intersectionPoint = new PointF(0, 0);
				return LineSegments.DoNotIntersect;
			}

			/* Line segments intersect: compute intersection point. 
			 */

			denom = a1 * b2 - a2 * b1;
			if (denom == 0)
			{
				intersectionPoint = new PointF(0, 0);
				return LineSegments.Colinear;
			}

			offset = denom < 0 ? -denom / 2 : denom / 2;

			/* The denom/2 is to get rounding instead of truncating.  It
			 * is added or subtracted to the numerator, depending upon the
			 * sign of the numerator.
			 */

			num = b1 * c2 - b2 * c1;
			float x = (num < 0 ? num - offset : num + offset) / denom;

			num = a2 * c1 - a1 * c2;
			float y = (num < 0 ? num - offset : num + offset) / denom;

			intersectionPoint = new PointF(x, y);

			return LineSegments.Intersect;
		}

		/// <summary>
		/// Given two points (current and last), returns a <see cref="SizeF"/> 
		/// object representing the delta in x and y between the 2 points.
		/// </summary>
		public static SizeF CalculatePositionDelta(PointF lastPoint, PointF currentPoint)
		{
			float deltaX = currentPoint.X - lastPoint.X;
			float deltaY = currentPoint.Y - lastPoint.Y;

			return new SizeF(deltaX, deltaY);
		}
	}
}
