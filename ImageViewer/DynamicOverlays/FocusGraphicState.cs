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

			if (base.StatefulGraphic.HitTest(e))
			{
				base.StatefulGraphic.State = base.StatefulGraphic.CreateFocusSelectedState(); 
				base.StatefulGraphic.State.OnMouseDown(e);

				return true;
			}

			//We should never actually get to here, but if we did, this should happen.
			base.StatefulGraphic.State = base.StatefulGraphic.CreateInactiveState();
			return false;
		}

		public override bool OnMouseMove(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

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
