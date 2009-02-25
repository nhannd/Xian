using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	internal class CreateTextOnlyGraphicState : CreateGraphicState
	{
		public CreateTextOnlyGraphicState(TextCalloutGraphic textCalloutGraphic) : base(textCalloutGraphic) {}

		public static CreateTextOnlyGraphicState Create(TextCalloutGraphic textCalloutGraphic)
		{
			return new CreateTextOnlyGraphicState(textCalloutGraphic);
		}

		internal new TextCalloutGraphic StatefulGraphic
		{
			get { return (TextCalloutGraphic) base.StatefulGraphic; }
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			this.StatefulGraphic.CoordinateSystem = CoordinateSystem.Destination;
			this.StatefulGraphic.PointOfInterest = mouseInformation.Location;
			this.StatefulGraphic.TextLocation = mouseInformation.Location;
			this.StatefulGraphic.ResetCoordinateSystem();
			this.StatefulGraphic.State = this.StatefulGraphic.CreateFocussedSelectedState();
			this.StatefulGraphic.Callout.StartEdit();
			return true;
		}
	}
}