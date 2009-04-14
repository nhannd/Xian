using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	internal class InteractiveTextAreaBuilder : InteractiveTextGraphicBuilder
	{
		public InteractiveTextAreaBuilder(ITextGraphic textGraphic) : base(textGraphic) {}

		internal new ITextGraphic Graphic
		{
			get { return (ITextGraphic) base.Graphic; }
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			this.Graphic.CoordinateSystem = CoordinateSystem.Destination;
			this.Graphic.Location = mouseInformation.Location;
			this.Graphic.ResetCoordinateSystem();
			this.NotifyGraphicComplete();
			return true;
		}
	}
}