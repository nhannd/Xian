using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	public class FocussedGraphicState : StandardGraphicState
	{
		public FocussedGraphicState(IStandardStatefulGraphic standardStatefulGraphic)
			: base(standardStatefulGraphic)
		{
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			if (this.StandardStatefulGraphic.HitTest(mouseInformation.Location))
			{
				this.StandardStatefulGraphic.State = this.StandardStatefulGraphic.CreateFocussedSelectedState();
				this.StandardStatefulGraphic.State.Start(mouseInformation);

				return true;
			}

			//We should never actually get to here, but if we did, this should happen.
			base.StatefulGraphic.State = this.StandardStatefulGraphic.CreateInactiveState();
			return false;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			if (!this.StandardStatefulGraphic.HitTest(mouseInformation.Location))
			{
				this.StandardStatefulGraphic.State = this.StandardStatefulGraphic.CreateInactiveState();
				return false;
			}

			return true;
		}

		public override string ToString()
		{
			return "FocusGraphicState\n";
		}
	}
}
