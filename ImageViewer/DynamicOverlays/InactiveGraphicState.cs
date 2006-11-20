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

		public override bool Track(IMouseInformation mouseInformation)
		{
			// If mouse is over object, transition to focused state
			if (base.StatefulGraphic.HitTest(mouseInformation.Location))
			{
				base.StatefulGraphic.State = base.StatefulGraphic.CreateFocusState();
				return true;
			}

			return false;
		}

		public override void OnEnterState(IMouseInformation mouseInformation)
		{
			base.StatefulGraphic.OnEnterInactiveState(mouseInformation);
		}

		public override string ToString()
		{
			return "InactiveGraphicState\n";
		}
	}
}
