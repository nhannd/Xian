#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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

namespace ClearCanvas.ImageViewer.Mathematics
{
	/// <summary>
	/// A collection of utility methods that operation on rectangles.
	/// </summary>
	public static class RectangleUtilities
	{
		/// <summary>
		/// Computes the intersection between two rectangles.
		/// </summary>
		/// <param name="primaryRectangle"></param>
		/// <param name="secondaryRectangle"></param>
		/// <returns></returns>
		/// <remarks>
		/// .NET's Rectangle class does not properly compute intersections
		/// between rectangles when either the height and/or width is negative.
		///
		/// NOTE: the order in which rectangles are passed into this function is IMPORTANT!
		/// The function maintains the sense/direction (of the width and height) of the first
		/// rectangle passed in.
		/// </remarks>
		public static RectangleF Intersect(RectangleF primaryRectangle, RectangleF secondaryRectangle)
		{
			if (!DoesIntersect(primaryRectangle, secondaryRectangle))
				return Rectangle.Empty;

			float left, top, right, bottom;

			if (primaryRectangle.Width >= 0)
			{
				if (secondaryRectangle.Width >= 0)
				{
					left = Math.Max(primaryRectangle.Left, secondaryRectangle.Left);
					right = Math.Min(primaryRectangle.Right, secondaryRectangle.Right);
				}
				else
				{
					left = Math.Max(primaryRectangle.Left, secondaryRectangle.Right);
					right = Math.Min(primaryRectangle.Right, secondaryRectangle.Left);
				}
			}
			else
			{
				if (secondaryRectangle.Width >= 0)
				{
					left = Math.Min(primaryRectangle.Left, secondaryRectangle.Right);
					right = Math.Max(primaryRectangle.Right, secondaryRectangle.Left);
				}
				else
				{
					left = Math.Min(primaryRectangle.Left, secondaryRectangle.Left);
					right = Math.Max(primaryRectangle.Right, secondaryRectangle.Right);
				}
			}

			if (primaryRectangle.Height >= 0)
			{
				if (secondaryRectangle.Height >= 0)
				{
					top = Math.Max(primaryRectangle.Top, secondaryRectangle.Top);
					bottom = Math.Min(primaryRectangle.Bottom, secondaryRectangle.Bottom);
				}
				else
				{
					top = Math.Max(primaryRectangle.Top, secondaryRectangle.Bottom);
					bottom = Math.Min(primaryRectangle.Bottom, secondaryRectangle.Top);
				}
			}
			else
			{
				if (secondaryRectangle.Height >= 0)
				{
					top = Math.Min(primaryRectangle.Top, secondaryRectangle.Bottom);
					bottom = Math.Max(primaryRectangle.Bottom, secondaryRectangle.Top);
				}
				else
				{
					top = Math.Min(primaryRectangle.Top, secondaryRectangle.Top);
					bottom = Math.Max(primaryRectangle.Bottom, secondaryRectangle.Bottom);
				}
			}

			return RectangleF.FromLTRB(left, top, right, bottom);
		}

		/// <summary>
		/// Rounds the 4 sides (top, left, right, bottom) to integer values, but
		/// it does not do typical float-style rounding.  The values are rounded
		/// away from the center of the rectangle.
		/// </summary>
		/// <param name="rect"></param>
		/// <returns>A new rectangle that has been rounded away from the centre
		/// of the original input rectangle.</returns>
		public static RectangleF RoundInflate(RectangleF rect)
		{
			float left, top, right, bottom;

			if (rect.Width >= 0)
			{
				if (rect.Left >= 0)
				{
					left = (int)rect.Left;
					right = (int)(rect.Right + 0.5);
				}
				else
				{
					left = (int)(rect.Left - 0.5);

					if (rect.Right >= 0)
						right = (int)(rect.Right + 0.5);
					else
						right = (int)rect.Right;
				}
			}
			else
			{
				if (rect.Left >= 0)
				{
					left = (int)(rect.Left + 0.5);
					if (rect.Right >= 0)
						right = (int)rect.Right;
					else
						right = (int)(rect.Right - 0.5);
				}
				else
				{
					left = (int)rect.Left;
					right = (int)(rect.Right - 0.5);
				}
			}

			if (rect.Height >= 0)
			{
				if (rect.Top >= 0)
				{
					top = (int)rect.Top;
					bottom = (int)(rect.Bottom + 0.5);
				}
				else
				{
					top = (int)(rect.Top - 0.5);

					if (rect.Bottom >= 0)
						bottom = (int)(rect.Bottom + 0.5);
					else
						bottom = (int)rect.Bottom;
				}
			}
			else
			{
				if (rect.Top >= 0)
				{
					top = (int)(rect.Top + 0.5);
					if (rect.Bottom >= 0)
						bottom = (int)rect.Bottom;
					else
						bottom = (int)(rect.Bottom - 0.5);
				}
				else
				{
					top = (int)rect.Top;
					bottom = (int)(rect.Bottom - 0.5);
				}
			}

			return RectangleF.FromLTRB(left, top, right, bottom);
		}

		/// <summary>
		/// Determines if the two input rectangles intersect at all.
		/// </summary>
		/// <param name="r1"></param>
		/// <param name="r2"></param>
		/// <returns></returns>
		public static bool DoesIntersect(RectangleF r1, RectangleF r2)
		{
			bool intersectLeftRight;
			bool intersectTopBottom;

			if (r1.Width >= 0)
			{
				if (r2.Width >= 0)
					intersectLeftRight = !((r1.Right <= r2.Left) || (r2.Right <= r1.Left));
				else
					intersectLeftRight = !((r1.Right <= r2.Right) || (r2.Left <= r1.Left));
			}
			else
			{
				if (r2.Width >= 0)
					intersectLeftRight = !((r1.Left <= r2.Left) || (r2.Right <= r1.Right));
				else
					intersectLeftRight = !((r1.Left <= r2.Right) || (r2.Left <= r1.Right));
			}

			if (r1.Height >= 0)
			{
				if (r2.Height >= 0)
					intersectTopBottom = !((r1.Bottom <= r2.Top) || (r2.Bottom <= r1.Top));
				else
					intersectTopBottom = !((r1.Bottom <= r2.Bottom) || (r2.Top <= r1.Top));
			}
			else
			{
				if (r2.Height >= 0)
					intersectTopBottom = !((r1.Top <= r2.Top) || (r2.Bottom <= r1.Bottom));
				else
					intersectTopBottom = !((r1.Top <= r2.Bottom) || (r2.Top <= r1.Bottom));
			}

			return intersectLeftRight && intersectTopBottom;
		}

		/// <summary>
		/// Shrinks the bottom and right coordinates of a rectangle by 1.
		/// </summary>
		/// <param name="rect"></param>
		/// <returns></returns>
		/// <remarks>
		/// When using a Rectangle's left, top, right, bottom paramters as coordinates,
		/// (say, when identifying pixel location in an image) it is often useful for the
		/// right and bottom parameters to be exactly one less.  For example, if a Bitmap
		/// is 10x10 and represented by Rectangle(0,0,10,10) calling Bitmap.GetPixel(right, bottom)
		/// won't work.  However, it does work if the bitmap is represented by Rectangle(0,0,9,9).
		/// Note that if the width and/or height of the rectangle is negative, this method
		/// will shrink the left and/or top values by one instead.
		/// </remarks>
		public static Rectangle MakeRectangleZeroBased(Rectangle rect)
		{
			if (rect.IsEmpty)
				return Rectangle.Empty;

			int left, top, right, bottom;

			if (rect.Width >= 0)
			{
				left = rect.Left;
				right = rect.Right - 1;
			}
			else
			{
				left = rect.Left - 1;
				right = rect.Right;
			}

			if (rect.Height >= 0)
			{
				top = rect.Top;
				bottom = rect.Bottom - 1;
			}
			else
			{
				top = rect.Top - 1;
				bottom = rect.Bottom;
			}

			return Rectangle.FromLTRB(left, top, right, bottom);
		}

		/// <summary>
		/// Computes the bounding rectangle of a collection of points.
		/// </summary>
		/// <param name="points">a collection of points.</param>
		/// <returns>the bounding rectangle of all the points.</returns>
		/// <exception cref="NullReferenceException">if the input array is null.</exception>
		/// <exception cref="ArgumentException">if the input array is empty.</exception>
		public static RectangleF ComputeBoundingRectangle(params PointF[] points)
		{
			Platform.CheckForNullReference(points, "points");
			Platform.CheckPositive(points.Length, "points.Length");

			PointF topLeft = points[0], bottomRight = points[0];

			for (int i = 1; i < points.Length; ++i)
			{
				PointF point = points[i];
				topLeft.X = Math.Min(topLeft.X, point.X);
				topLeft.Y = Math.Min(topLeft.Y, point.Y);
				bottomRight.X = Math.Max(bottomRight.X, point.X);
				bottomRight.Y = Math.Max(bottomRight.Y, point.Y);
			}

			return RectangleF.FromLTRB(topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);
		}

		/// <summary>
		/// Returns a value indicating whether the specified rectangle is normalized.
		/// </summary>
		/// <param name="rectangle"></param>
		/// <returns></returns>
		public static bool IsRectangleNormalized(RectangleF rectangle)
		{
			return (!(FloatComparer.IsLessThan(rectangle.Left, 0.0f) ||
					FloatComparer.IsGreaterThan(rectangle.Left, 1.0f) ||
					FloatComparer.IsLessThan(rectangle.Right, 0.0f) ||
					FloatComparer.IsGreaterThan(rectangle.Right, 1.0f) ||
					FloatComparer.IsLessThan(rectangle.Top, 0.0f) ||
					FloatComparer.IsGreaterThan(rectangle.Top, 1.0f) ||
					FloatComparer.IsLessThan(rectangle.Bottom, 0.0f) ||
					FloatComparer.IsGreaterThan(rectangle.Bottom, 1.0f) ||
					FloatComparer.IsGreaterThan(rectangle.Left, rectangle.Right) ||
					FloatComparer.IsGreaterThan(rectangle.Top, rectangle.Bottom)));
		}
		/// <summary>
		/// Verifies that the specified rectangle is normalized.
		/// </summary>
		/// <exception cref="ArgumentException">Thrown when the rectangle is not normalized.</exception>
		public static void VerifyNormalizedRectangle(RectangleF rectangle)
		{ 
			if (!IsRectangleNormalized(rectangle))
				throw new ArgumentException(String.Format(SR.ExceptionInvalidNormalizedRectangle, rectangle.Top.ToString(), rectangle.Left.ToString(), rectangle.Bottom.ToString(), rectangle.Right.ToString()));
		}

		/// <summary>
		/// Calculates a sub-rectangle of the <paramref name="parentRectangle"/> given a 
		/// normalized rectangle (see <paramref name="normalizedRectangle"/>).
		/// </summary>
		public static Rectangle CalculateSubRectangle(Rectangle parentRectangle, RectangleF normalizedRectangle)
		{
			VerifyNormalizedRectangle(normalizedRectangle);

			int left = parentRectangle.Left + (int)(normalizedRectangle.Left * parentRectangle.Width);
			int right = parentRectangle.Left + (int)(normalizedRectangle.Right * parentRectangle.Width);
			int top = parentRectangle.Top + (int)(normalizedRectangle.Top * parentRectangle.Height);
			int bottom = parentRectangle.Top + (int)(normalizedRectangle.Bottom * parentRectangle.Height);

			return new Rectangle(left, top, right - left, bottom - top);
		}
	}
}
