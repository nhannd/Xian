using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	public class InactiveGraphicState : StandardGraphicState
	{
		public InactiveGraphicState(IStandardStatefulGraphic standardStatefulGraphic)
			: base(standardStatefulGraphic)
		{
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			// If mouse is over object, transition to focused state
			if (this.StandardStatefulGraphic.HitTest(mouseInformation.Location))
			{
				this.StandardStatefulGraphic.State = this.StandardStatefulGraphic.CreateFocussedState();
				return true;
			}

			return false;
		}

		public override string ToString()
		{
			return "InactiveGraphicState\n";
		}
	}
}
