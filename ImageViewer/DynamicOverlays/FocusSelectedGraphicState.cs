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

		public override bool Track(MouseInformation pointerInformation)
		{
			base.StatefulGraphic.SetCursorToken(pointerInformation); 

			if (!base.StatefulGraphic.HitTest(pointerInformation.Point))
			{
				base.StatefulGraphic.State = base.StatefulGraphic.CreateSelectedState();
				return false;
			}

			return true;
		}

		public override bool Start(MouseInformation pointerInformation)
		{
			// User has clicked the graphic body
			if (base.StatefulGraphic.HitTest(pointerInformation.Point))
			{
				base.StatefulGraphic.State = new MoveGraphicState(base.StatefulGraphic);
				base.StatefulGraphic.State.Start(pointerInformation);
				return true;
			}

			//We should never actually get to here, but if we did, this should happen.
			base.StatefulGraphic.State = base.StatefulGraphic.CreateSelectedState();
			return false;
		}

		public override void OnEnterState(MouseInformation pointerInformation)
		{
			base.StatefulGraphic.OnEnterFocusSelectedState(pointerInformation);
		}

		public override string ToString()
		{
			return "FocusSelectedGraphicState\n";
		}
	}
}
