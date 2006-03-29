using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace ClearCanvas.Common.Mathematics
{
	public static class Vector
	{
		public static double Distance(Point pt1, Point pt2)
		{
			int deltaX = pt2.X - pt1.X;
			int deltaY = pt2.Y - pt1.Y;

			return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
		}

		public static double Distance(PointF pt1, PointF pt2)
		{
			float deltaX = pt2.X - pt1.X;
			float deltaY = pt2.Y - pt1.Y;

			return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
		}

		public static PointF Midpoint(PointF pt1, PointF pt2)
		{
			float x = (pt1.X + pt2.X) / 2;
			float y = (pt1.Y + pt2.Y) / 2;

			return new PointF(x, y);
		}

		public static Point Midpoint(Point pt1, Point pt2)
		{
			int x = (pt1.X + pt2.X) / 2;
			int y = (pt1.Y + pt2.Y) / 2;

			return new Point(x, y);
		}

		public enum LineSegments
		{
			DoNotIntersect,
			Intersect,
			Colinear
		}

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
	}
}
