using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.Tools.Standard {
	internal class CreateTextCalloutGraphicState : CreateGraphicState {
		private int clickIndex = 0;

		public CreateTextCalloutGraphicState(TextCalloutGraphic textCalloutGraphic) : base(textCalloutGraphic) { }

		internal new TextCalloutGraphic StatefulGraphic
		{
			get { return (TextCalloutGraphic)base.StatefulGraphic; }
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			if (clickIndex == 0)
			{
				this.StatefulGraphic.CoordinateSystem = CoordinateSystem.Destination;
				this.StatefulGraphic.PointOfInterest = mouseInformation.Location;
				this.StatefulGraphic.TextLocation = mouseInformation.Location;
				this.StatefulGraphic.ResetCoordinateSystem();
			}
			else if (clickIndex == 1)
			{
				this.StatefulGraphic.State = this.StatefulGraphic.CreateFocussedSelectedState();
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
				this.StatefulGraphic.CoordinateSystem = CoordinateSystem.Destination;
				this.StatefulGraphic.TextLocation = mouseInformation.Location;
				this.StatefulGraphic.ResetCoordinateSystem();
				this.StatefulGraphic.Draw();
			}

			return true;
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			return true;
		}
	}
}
