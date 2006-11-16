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

		public override bool Start(MouseInformation pointerInformation)
		{
			if (base.StatefulGraphic.HitTest(pointerInformation.Point))
			{
				base.StatefulGraphic.State = base.StatefulGraphic.CreateFocusSelectedState();
				base.StatefulGraphic.State.Start(pointerInformation);

				return true;
			}

			//We should never actually get to here, but if we did, this should happen.
			base.StatefulGraphic.State = base.StatefulGraphic.CreateInactiveState();
			return false;
		}

		public override bool Track(MouseInformation pointerInformation)
		{
			base.StatefulGraphic.SetCursorToken(pointerInformation);

			if (!base.StatefulGraphic.HitTest(pointerInformation.Point))
			{
				base.StatefulGraphic.State = base.StatefulGraphic.CreateInactiveState();
				return false;
			}

			return true;
		}

		public override void OnEnterState(MouseInformation pointerInformation)
		{
			base.StatefulGraphic.OnEnterFocusState(pointerInformation);
		}

		public override string ToString()
		{
			return "FocusGraphicState\n";
		}
	}
}
