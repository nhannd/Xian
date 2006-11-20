using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.DynamicOverlays
{
	public class FocusGraphicState : GraphicState
	{
		public FocusGraphicState(StatefulGraphic statefulGraphic)
			: base(statefulGraphic)
		{
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			if (base.StatefulGraphic.HitTest(mouseInformation.Location))
			{
				base.StatefulGraphic.State = base.StatefulGraphic.CreateFocusSelectedState();
				base.StatefulGraphic.State.Start(mouseInformation);

				return true;
			}

			//We should never actually get to here, but if we did, this should happen.
			base.StatefulGraphic.State = base.StatefulGraphic.CreateInactiveState();
			return false;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			if (!base.StatefulGraphic.HitTest(mouseInformation.Location))
			{
				base.StatefulGraphic.State = base.StatefulGraphic.CreateInactiveState();
				return false;
			}

			return true;
		}

		public override void OnEnterState(IMouseInformation mouseInformation)
		{
			base.StatefulGraphic.OnEnterFocusState(mouseInformation);
		}

		public override string ToString()
		{
			return "FocusGraphicState\n";
		}
	}
}
