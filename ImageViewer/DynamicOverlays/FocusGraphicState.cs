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

			// If mouse is not over the object, transition back to inactive state
			if (!base.StatefulGraphic.HitTest(e))
			{
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
