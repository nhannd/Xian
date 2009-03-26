using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	internal class InteractiveTextCalloutBuilder : InteractiveGraphicBuilder
	{
		private int clickIndex = 0;

		public InteractiveTextCalloutBuilder(UserCalloutGraphic textCalloutGraphic) : base(textCalloutGraphic) { }

		internal new UserCalloutGraphic Graphic
		{
			get { return (UserCalloutGraphic)base.Graphic; }
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			if (clickIndex == 0)
			{
				this.Graphic.CoordinateSystem = CoordinateSystem.Destination;
				this.Graphic.EndPoint = mouseInformation.Location;
				this.Graphic.Location = mouseInformation.Location;
				this.Graphic.ResetCoordinateSystem();
			}
			else if (clickIndex == 1)
			{
				this.NotifyGraphicComplete();
			}
			else
			{
				return false;
			}

			clickIndex++;
			return true;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			if (clickIndex == 1)
			{
				this.Graphic.CoordinateSystem = CoordinateSystem.Destination;
				this.Graphic.Location = mouseInformation.Location;
				this.Graphic.ResetCoordinateSystem();
				this.Graphic.Draw();
			}

			return true;
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			return true;
		}
	}
}