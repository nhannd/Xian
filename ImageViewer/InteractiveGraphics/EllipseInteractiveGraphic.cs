using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Text;
using ClearCanvas.ImageViewer.Graphics;
using System.Drawing;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// A interactive elliptical graphic.
	/// </summary>
	public class EllipseInteractiveGraphic : BoundableInteractiveGraphic
	{
		/// <summary>
		/// Initializes a new instance of <see cref="RectangleInteractiveGraphic"/>.
		/// </summary>
		/// <param name="userCreated">Indicates whether the graphic was created
		/// through user interaction.</param>
		public EllipseInteractiveGraphic(bool userCreated)
			: base(userCreated)
		{
		}

		protected override BoundableGraphic CreateBoundableGraphic()
		{
			return new EllipsePrimitive();
		}

		public override bool HitTest(System.Drawing.Point point)
		{
			this.CoordinateSystem = CoordinateSystem.Destination;
			RectangleF boundingBox = new RectangleF(this.Left, this.Top, this.Width, this.Height);

			GraphicsPath path = new GraphicsPath();
			path.AddRectangle(RectangleUtilities.ConvertToPositiveRectangle(boundingBox));

			Pen pen = new Pen(Brushes.White, VectorGraphic.HitTestDistance);
			bool result = path.IsOutlineVisible(point, pen);

			path.Dispose();
			pen.Dispose();
			this.ResetCoordinateSystem();

			return result || base.HitTest(point);
		}

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

		/// <summary>
		/// Finds the intersection between an ellipse and a line that starts at the
		/// center of the ellipse and ends at an aribtrary point.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="center"></param>
		/// <param name="point"></param>
		/// <returns></returns>
		internal static PointF IntersectEllipseAndLine(float a, float b, PointF center, PointF point)
		{
			// Test point
			float x2 = point.X;
			float y2 = point.Y;

			float a2 = a * a;
			float b2 = b * b;

			// Center of ellipse
			float x1 = center.X;
			float y1 = center.Y;

			if (a == 0 || b == 0)
				return new PointF(x1, y1);

			if (x2 == x1)
			{
				if (y2 > y1)
					return new PointF(x1, y1 + b);
				else
					return new PointF(x1, y1 - b);
			}

			// Rise and run
			float dy = y2 - y1;
			float dx = x2 - x1;

			// Slope
			float m = dy / dx;
			float m2 = m * m;

			// y-intercept
			float c = y1 - m * x1;
			float c2 = c * c;

			float A = (m2 / b2) + (1 / a2);
			float B = 2 * ((m / b2) * (c - y1) - (x1 / a2));
			float C = (c2 - (2 * c * y1) + (y1 * y1)) / b2 + ((x1 * x1) / a2) - 1;

			float x;

			if (x2 >= x1)
				x = (-B + (float)Math.Sqrt(B * B - 4 * A * C)) / (2 * A);
			else
				x = (-B - (float)Math.Sqrt(B * B - 4 * A * C)) / (2 * A);

			float y = m * x + c;

			return new PointF(x, y);
		}

	}
}
