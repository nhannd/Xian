using System;
using System.Drawing;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.DynamicOverlays
{
	public class FocusSelectedGraphicState : GraphicState
	{
		public FocusSelectedGraphicState(StatefulGraphic graphic)
			: base(graphic)
		{
		}

		public override bool OnMouseMove(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			if (!base.StatefulGraphic.HitTest(e))
			{
				base.StatefulGraphic.State = base.StatefulGraphic.CreateSelectedState();
				return false;
			}

			return true;
		}

		public override bool OnMouseDown(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			// User has clicked the graphic body
			if (base.StatefulGraphic.HitTest(e))
			{
				base.StatefulGraphic.State = new MoveGraphicState(base.StatefulGraphic);
				base.StatefulGraphic.State.OnMouseDown(e);
				return true;
			}

			//We should never actually get to here, but if we did, this should happen.
			base.StatefulGraphic.State = base.StatefulGraphic.CreateSelectedState();
			return false;
		}

		public override bool OnMouseUp(XMouseEventArgs e)
		{
			// Must return true.  If we just go with the base class
			// implementation which returns false, PresentationImage.OnMouseUp
			// will think we haven't handled this and then route the MouseUp message
			// to some other mouse tool, which we don't want.
			return true;
		}

		public override void OnEnterState(XMouseEventArgs e)
		{
			base.StatefulGraphic.OnEnterFocusSelectedState(e);
		}

		public override string ToString()
		{
			return "FocusSelectedGraphicState\n";
		}
	}
}
