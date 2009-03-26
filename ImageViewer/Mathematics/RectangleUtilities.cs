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
using System.Collections.Generic;
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
		/// <param name="points">A collection of points.</param>
		/// <returns>The bounding rectangle of all the points.</returns>
		/// <exception cref="NullReferenceException">If the input array is null.</exception>
		/// <exception cref="ArgumentException">If the input array is empty.</exception>
		public static RectangleF ComputeBoundingRectangle(params PointF[] points)
		{
			return ComputeBoundingRectangle((IEnumerable<PointF>) points);
		}

		/// <summary>
		/// Computes the bounding rectangle of a collection of rectangle.
		/// </summary>
		/// <param name="rectangles">A collection of rectangles.</param>
		/// <returns>The bounding rectangle that encompasses all the rectangles.</returns>
		/// <exception cref="NullReferenceException">If the input collection is null.</exception>
		/// <exception cref="ArgumentException">If the input collection is empty.</exception>
		public static RectangleF ComputeBoundingRectangle(IEnumerable<RectangleF> rectangles)
		{
			Platform.CheckForNullReference(rectangles, "rectangles");

			List<PointF> points = new List<PointF>();
			foreach (RectangleF rectangle in rectangles)
			{
				points.Add(new PointF(rectangle.Left, rectangle.Top));
				points.Add(new PointF(rectangle.Right, rectangle.Bottom));
			}
			return ComputeBoundingRectangle(points);
		}

		/// <summary>
		/// Computes the bounding rectangle of a collection of points.
		/// </summary>
		/// <param name="points">A collection of points.</param>
		/// <returns>The bounding rectangle of all the points.</returns>
		/// <exception cref="NullReferenceException">If the input collection is null.</exception>
		/// <exception cref="ArgumentException">If the input collection is empty.</exception>
		public static RectangleF ComputeBoundingRectangle(IEnumerable<PointF> points)
		{
			Platform.CheckForNullReference(points, "points");

			bool initialized = false;
			PointF topLeft = PointF.Empty, bottomRight = PointF.Empty;

			foreach(PointF point in points)
			{
				if(!initialized)
				{
					topLeft = bottomRight = point;
					initialized = true;
					continue;
				}

				topLeft.X = Math.Min(topLeft.X, point.X);
				topLeft.Y = Math.Min(topLeft.Y, point.Y);
				bottomRight.X = Math.Max(bottomRight.X, point.X);
				bottomRight.Y = Math.Max(bottomRight.Y, point.Y);
			}

			Platform.CheckTrue(initialized, "At least one point.");

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

		/// <summary>
		/// Calculates the normalized rectangle of <paramref name="subRectangle"/> within <paramref name="parentRectangle"/>.
		/// </summary>
		public static RectangleF CalculateNormalizedSubRectangle(Rectangle parentRectangle, Rectangle subRectangle)
		{
			float left = (subRectangle.Left - parentRectangle.Left) / (float)parentRectangle.Width;
			float right = (subRectangle.Right - parentRectangle.Left) / (float)parentRectangle.Width;
			float top = (subRectangle.Top - parentRectangle.Top) / (float)parentRectangle.Height;
			float bottom = (subRectangle.Bottom - parentRectangle.Top) / (float)parentRectangle.Height;

			RectangleF normalizedRectangle = new RectangleF(left, top, right - left, bottom - top);
			VerifyNormalizedRectangle(normalizedRectangle);
			return normalizedRectangle;
		}

		/// <summary>
		/// Takes a rectangle whose width and/or height may be negative and converts it to
		/// an equivalent rectangle whose width and height are guaranteed to be positive
		/// </summary>
		/// <param name="rectangle"></param>
		/// <returns></returns>
		/// <remarks>
		/// GDI methods always require rectangles with non-negative widths and heights. Use
		/// this method when making such calls.
		/// </remarks>
		public static RectangleF ConvertToPositiveRectangle(RectangleF rectangle)
		{
			float left = rectangle.Left;
			float top = rectangle.Top;
			float width = rectangle.Width;
			float height = rectangle.Height;

			if (rectangle.Width < 0)
			{
				left = rectangle.Left + rectangle.Width;
				width = -rectangle.Width;
			}

			if (rectangle.Height < 0)
			{
				top = rectangle.Top + rectangle.Height;
				height = -rectangle.Height;
			}

			return new RectangleF(left, top, width, height);
		}

		/// <summary>
		/// Computes a rectangle specifying the 1-based pixel address and the row and column offset to get the pixel address of the opposite corner
		/// given a rectangle specifying the 0-based coordinate and size (whose width and height need not be positive).
		/// </summary>
		/// <param name="rectangle">The 0-based rectangle specified as a 0-based coordinate and a size.</param>
		/// <returns>An equivalent rectangle specifying the 1-based pixel address of the coordinate and row and column offset to get the pixel address of the opposite corner.</returns>
		/// <exception cref="ArgumentException">Thrown if the given rectangle has zero-area, as it cannot be represented in a 1-based pixel address rectangle.</exception>
		/// <remarks>
		/// <para>In DICOM and certain other applications, areas of images are identified with the first pixel being at position row 1 and column 1.
		/// In most Windows graphics rendering systems, imaging coordinates are identified as an infinitesimal point at the top-left corner of a given pixel
		/// and the coordinates are given in a 0-based system (that is, the first pixel has a coordinate of (0,0)).</para>
		/// <para>It is trivial to compute the addresses of the pixels included within the rectangle when the rectangle is positively-oriented
		/// (has positive width and height) since the left most pixels are in column <see cref="Rectangle.X"/>+1, the right most pixels are in
		/// column <see cref="Rectangle.X"/>+<see cref="Rectangle.Width"/>, and so on. It is somewhat more complicated to compute when the
		/// rectangle is not positively-oriented due to the singularities when either <see cref="Rectangle.Width"/> or <see cref="Rectangle.Height"/>
		/// is 0 (and hence rectangles having zero-area cannot be specified as a pixel address rectangle - a pixel address rectangle having size
		/// 0x0 is considered as containing 1 row and 1 column).</para>
		/// </remarks>
		public static Rectangle ConvertToPixelAddressRectangle(Rectangle rectangle)
		{
			if(rectangle.Width * rectangle.Height == 0)
				throw new ArgumentException("Zero-area rectangles cannot be specified in terms of a pixel address rectangle.", "rectangle");

			Size locationOffset = new Size(rectangle.Width > 0 ? 1 : 0, rectangle.Height > 0 ? 1 : 0);
			Size sizeOffset = new Size(rectangle.Width > 0 ? -1 : 1, rectangle.Height > 0 ? -1 : 1);

			return new Rectangle(rectangle.Location + locationOffset, rectangle.Size + sizeOffset);
		}

		/// <summary>
		/// Computes a rectangle from a centroid and a set of dimensions.
		/// </summary>
		/// <param name="centroid">The centroid of the rectangle.</param>
		/// <param name="dimensions">The dimensions of the rectangle.</param>
		/// <returns>A rectangle of given dimensions centered around the centroid.</returns>
		public static Rectangle ConvertToRectangle(Point centroid, Size dimensions)
		{
			return Rectangle.Round(ConvertToRectangle((PointF) centroid, dimensions));
		}

		/// <summary>
		/// Computes a rectangle from a centroid and a set of dimensions.
		/// </summary>
		/// <param name="centroid">The centroid of the rectangle.</param>
		/// <param name="dimensions">The dimensions of the rectangle.</param>
		/// <returns>A rectangle of given dimensions centered around the centroid.</returns>
		public static RectangleF ConvertToRectangle(PointF centroid, SizeF dimensions)
		{
			float halfWidth = dimensions.Width/2f;
			float halfHeight = dimensions.Height/2f;
			return RectangleF.FromLTRB(centroid.X - halfWidth, centroid.Y - halfHeight, centroid.X + halfWidth, centroid.Y + halfHeight);
		}
	}
}
