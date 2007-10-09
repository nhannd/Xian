using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	public class FocussedSelectedGraphicState : StandardGraphicState
	{
		public FocussedSelectedGraphicState(IStandardStatefulGraphic standardStatefulGraphic)
			: base(standardStatefulGraphic)
		{
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			if (!this.StandardStatefulGraphic.HitTest(mouseInformation.Location))
			{
				this.StandardStatefulGraphic.State = this.StandardStatefulGraphic.CreateSelectedState();
				return false;
			}

			return true;
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			// User has clicked the graphic body
			if (this.StandardStatefulGraphic.HitTest(mouseInformation.Location))
			{
				this.StandardStatefulGraphic.State = new MoveGraphicState(this.StandardStatefulGraphic);
				this.StandardStatefulGraphic.State.Start(mouseInformation);
				return true;
			}

			//We should never actually get to here, but if we did, this should happen.
			this.StandardStatefulGraphic.State = this.StandardStatefulGraphic.CreateSelectedState();
			return false;
		}

		public override string ToString()
		{
			return "FocusSelectedGraphicState\n";
		}
	}
}
