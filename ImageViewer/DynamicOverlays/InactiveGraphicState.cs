using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.DynamicOverlays
{
	public class InactiveGraphicState : GraphicState
	{
		public InactiveGraphicState(StatefulGraphic statefulGraphic)
			: base(statefulGraphic)
		{
		}

		public override bool Track(MouseInformation pointerInformation)
		{
			// If mouse is over object, transition to focused state
			if (base.StatefulGraphic.HitTest(pointerInformation.Point))
			{
				base.StatefulGraphic.State = base.StatefulGraphic.CreateFocusState();
				return true;
			}

			return false;
		}

		public override void OnEnterState(MouseInformation pointerInformation)
		{
			base.StatefulGraphic.OnEnterInactiveState(pointerInformation);
		}

		public override string ToString()
		{
			return "InactiveGraphicState\n";
		}
	}
}
