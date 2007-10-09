using System.Drawing;

namespace ClearCanvas.ImageViewer.Mathematics
{
	/// <summary>
	/// Some simple, but useful, utilities for Point objects.
	/// </summary>
	public static class PointUtilities
	{
		/// <summary>
		/// Confines a point to be within the boundaries of a given rectangle.
		/// </summary>
		/// <param name="point">passed by ref so the point can be returned via the same variable.</param>
		/// <param name="rectangle">the rectangle in which to confine the point.</param>
		public static void ConfinePointToRectangle(ref Point point, Rectangle rectangle)
		{
			int x = point.X;
			int y = point.Y;

			ConfinePointToRectangle(ref x, ref y, rectangle);

			point.X = x;
			point.Y = y;
		}

		/// <summary>
		/// Confines a point (given x,y) to be within the boundaries of a given rectangle.
		/// </summary>
		/// <param name="x">The x coordinate of the point.</param>
		/// <param name="y">The y coordinate of the point.</param>
		/// <param name="rectangle">the rectangle in which to confine the point.</param>
		public static void ConfinePointToRectangle(ref int x, ref int y, Rectangle rectangle)
		{
			if (x < rectangle.Left)
				x = rectangle.Left;
			else if (x > rectangle.Right)
				x = rectangle.Right;

			if (y < rectangle.Top)
				y = rectangle.Top;
			else if (y > rectangle.Bottom)
				y = rectangle.Bottom;
		}
	}
}
