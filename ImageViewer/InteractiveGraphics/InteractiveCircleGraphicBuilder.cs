using System.Drawing;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	public class InteractiveCircleGraphicBuilder : InteractiveGraphicBuilder
	{
		private int _numberOfPointsAnchored = 1;
		private PointF _centre;

		public InteractiveCircleGraphicBuilder(IBoundableGraphic boundableGraphic) : base(boundableGraphic) {}

		public new IBoundableGraphic Graphic
		{
			get { return (IBoundableGraphic) base.Graphic; }
		}

		public override void Reset()
		{
			_numberOfPointsAnchored = 1;
			base.Reset();
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			// We just started creating
			if (_numberOfPointsAnchored == 1)
			{
				_centre = mouseInformation.Location;

				this.Graphic.CoordinateSystem = CoordinateSystem.Destination;
				this.Graphic.TopLeft = _centre;
				this.Graphic.BottomRight = _centre;
				this.Graphic.ResetCoordinateSystem();

				_numberOfPointsAnchored++;
			}
			// We're done creating
			else
			{
				this.NotifyGraphicComplete();
			}

			return true;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			this.Graphic.CoordinateSystem = CoordinateSystem.Destination;
			try
			{
				float radius = (float) Vector.Distance(_centre, mouseInformation.Location);
				SizeF offset = new SizeF(radius, radius);
				this.Graphic.TopLeft = _centre + offset;
				this.Graphic.BottomRight = _centre - offset;
			}
			finally
			{
				this.Graphic.ResetCoordinateSystem();
			}

			this.Graphic.Draw();
			return true;
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			return true;
		}
	}
}