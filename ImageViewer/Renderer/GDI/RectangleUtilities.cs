using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Renderer.GDI
{
	public class RectangleUtilities
	{
		/// <summary>
		/// Computes the intersection between two rectangles.
		/// </summary>
		/// <param name="r1"></param>
		/// <param name="r2"></param>
		/// <returns></returns>
		/// <remarks>
		/// .NET's Rectangle class does not properly compute intersections
		/// between rectangles when either the height and/or width is negative.
		/// This 
		/// </remarks>
		public static RectangleF Intersect(RectangleF r1,
					RectangleF r2)
		{
			if (!DoesIntersect(r1, r2))
				return Rectangle.Empty;

			float left, top, right, bottom;

			if (r1.Width >= 0)
			{
				if (r2.Width >= 0)
				{
					left = Math.Max(r1.Left, r2.Left);
					right = Math.Min(r1.Right, r2.Right);
				}
				else
				{
					left = Math.Max(r1.Left, r2.Right);
					right = Math.Min(r1.Right, r2.Left);
				}
			}
			else
			{
				if (r2.Width >= 0)
				{
					left = Math.Max(r1.Right, r2.Left);
					right = Math.Min(r1.Left, r2.Right);
				}
				else
				{
					left = Math.Max(r1.Right, r2.Right);
					right = Math.Min(r1.Left, r2.Left);
				}
			}

			if (r1.Height >= 0)
			{
				if (r2.Height >= 0)
				{
					top = Math.Max(r1.Top, r2.Top);
					bottom = Math.Min(r1.Bottom, r2.Bottom);
				}
				else
				{
					top = Math.Max(r1.Top, r2.Bottom);
					bottom = Math.Min(r1.Bottom, r2.Top);
				}
			}
			else
			{
				if (r2.Height >= 0)
				{
					top = Math.Max(r1.Bottom, r2.Top);
					bottom = Math.Min(r1.Top, r2.Bottom);
				}
				else
				{
					top = Math.Max(r1.Bottom, r2.Bottom);
					bottom = Math.Min(r1.Top, r2.Top);
				}
			}

			return RectangleF.FromLTRB(left, top, right, bottom);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rect1"></param>
		/// <param name="rect2"></param>
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
	}
}
