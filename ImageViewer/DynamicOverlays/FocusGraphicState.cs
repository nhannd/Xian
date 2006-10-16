using System;
using System.Drawing;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.DynamicOverlays
{
	public class FocusGraphicState : GraphicState
	{
		public FocusGraphicState(StatefulGraphic statefulGraphic)
			: base(statefulGraphic)
		{
		}

		public override bool OnMouseDown(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			// If user has clicked on the object, then transition to selected state
			if (base.StatefulGraphic.HitTest(e))
			{
				base.StatefulGraphic.State = base.StatefulGraphic.CreateSelectedState(); 
				base.StatefulGraphic.State.OnMouseDown(e);

				return true;
			}

			return false;
		}

		public override bool OnMouseMove(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			if (!base.StatefulGraphic.HitTest(e))
			{
				//Transition back to selected when focus is lost.
				if (base.StatefulGraphic.Selected)
					base.StatefulGraphic.State = base.StatefulGraphic.CreateSelectedState();
				else
					base.StatefulGraphic.State = base.StatefulGraphic.CreateInactiveState();

				return false;
			}

			return true;
		}

		public override void OnEnterState(XMouseEventArgs e)
		{
			base.StatefulGraphic.OnEnterFocusState(e);
		}

		public override string ToString()
		{
			return "FocusGraphicState\n";
		}
	}
}
