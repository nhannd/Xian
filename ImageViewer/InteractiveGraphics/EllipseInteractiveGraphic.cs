using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Text;
using ClearCanvas.ImageViewer.Graphics;
using System.Drawing;

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
			path.AddRectangle(boundingBox);

			Pen pen = new Pen(Brushes.White, VectorGraphic.HitTestDistance);
			bool result = path.IsOutlineVisible(point, pen);

			path.Dispose();
			pen.Dispose();
			this.ResetCoordinateSystem();

			return result || base.HitTest(point);
		}

		public override PointF GetClosestPoint(PointF point)
		{
			float a = this.Width/2;
			float b = this.Height/2;

			float centerX = this.Left + a;
			float centerY = this.Top + b;

			float rise = point.Y - centerY;
			float run = point.X - centerX;
			double theta;

			if (run != 0)
			{
				theta = Math.Atan(rise/run);

				if (run < 0)
					theta += Math.PI;

			}
			else
			{
				if (rise >= 0)
					theta = Math.PI / 2;
				else
					theta = -Math.PI / 2;
			}

			float x = centerX + a*(float)Math.Cos(theta);
			float y = centerY + b*(float)Math.Sin(theta);

			return new PointF(x,y);
		}
	}
}
