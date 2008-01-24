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

			int g;

			if (result)
				g = 4;

			path.Dispose();
			pen.Dispose();
			this.ResetCoordinateSystem();

			return result || base.HitTest(point);
		}
	}
}
