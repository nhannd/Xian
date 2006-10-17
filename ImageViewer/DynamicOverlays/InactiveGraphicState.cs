using System;
using System.Drawing;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.DynamicOverlays
{
	public class InactiveGraphicState : GraphicState
	{
		public InactiveGraphicState(StatefulGraphic statefulGraphic)
			: base(statefulGraphic)
		{
		}

		public override bool OnMouseMove(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			// If mouse is over object, transition to focused state
			if (base.StatefulGraphic.HitTest(e))
			{
				base.StatefulGraphic.State = base.StatefulGraphic.CreateFocusState();
				return true;
			}

			return false;
		}

		public override void OnEnterState(XMouseEventArgs e)
		{
			base.StatefulGraphic.OnEnterInactiveState(e);
		}

		public override string ToString()
		{
			return "InactiveGraphicState\n";
		}
	}
}
