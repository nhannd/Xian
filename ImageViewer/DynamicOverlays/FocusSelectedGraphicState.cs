using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.DynamicOverlays
{
	public class FocusSelectedGraphicState : GraphicState
	{
		public FocusSelectedGraphicState(StatefulGraphic graphic)
			: base(graphic)
		{
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			if (!base.StatefulGraphic.HitTest(mouseInformation.Location))
			{
				base.StatefulGraphic.State = base.StatefulGraphic.CreateSelectedState();
				return false;
			}

			return true;
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			// User has clicked the graphic body
			if (base.StatefulGraphic.HitTest(mouseInformation.Location))
			{
				base.StatefulGraphic.State = new MoveGraphicState(base.StatefulGraphic);
				base.StatefulGraphic.State.Start(mouseInformation);
				return true;
			}

			//We should never actually get to here, but if we did, this should happen.
			base.StatefulGraphic.State = base.StatefulGraphic.CreateSelectedState();
			return false;
		}

		public override void OnEnterState(IMouseInformation mouseInformation)
		{
			base.StatefulGraphic.OnEnterFocusSelectedState(mouseInformation);
		}

		public override string ToString()
		{
			return "FocusSelectedGraphicState\n";
		}
	}
}
