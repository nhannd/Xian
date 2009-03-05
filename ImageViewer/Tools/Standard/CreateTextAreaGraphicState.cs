using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	internal class CreateTextAreaGraphicState : CreateGraphicState
	{
		public CreateTextAreaGraphicState(TextAreaGraphic textAreaGraphic) : base(textAreaGraphic) {}

		internal new TextAreaGraphic StatefulGraphic
		{
			get { return (TextAreaGraphic) base.StatefulGraphic; }
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			this.StatefulGraphic.CoordinateSystem = CoordinateSystem.Destination;
			this.StatefulGraphic.Location = mouseInformation.Location;
			this.StatefulGraphic.ResetCoordinateSystem();
			this.StatefulGraphic.State = this.StatefulGraphic.CreateFocussedSelectedState();
			this.StatefulGraphic.StartEdit();
			return true;
		}
	}
}