using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.DynamicOverlays
{
	public class SelectedGraphicState : GraphicState
	{
		public SelectedGraphicState(StatefulGraphic statefulGraphic)
			: base(statefulGraphic)
		{
		}

		public override bool OnMouseMove(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			if (base.StatefulGraphic.HitTest(e))
			{
				base.StatefulGraphic.State = base.StatefulGraphic.CreateFocusSelectedState();
				return true;
			}

			return false;
		}

		public override bool OnMouseDown(XMouseEventArgs e)
		{
			if (!base.StatefulGraphic.HitTest(e))
			{
				base.StatefulGraphic.State = base.StatefulGraphic.CreateInactiveState();
				return false;
			}

			return true;
		}

		public override void OnEnterState(XMouseEventArgs e)
		{
			base.StatefulGraphic.OnEnterSelectedState(e);
		}

		public override string ToString()
		{
			return "SelectedGraphicState\n";
		}
	}
}
