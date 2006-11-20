using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using System.Drawing;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.DynamicOverlays
{
	public class SelectedGraphicState : GraphicState
	{
		public SelectedGraphicState(StatefulGraphic statefulGraphic)
			: base(statefulGraphic)
		{
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			if (base.StatefulGraphic.HitTest(mouseInformation.Location))
			{
				base.StatefulGraphic.State = base.StatefulGraphic.CreateFocusSelectedState();
				return true;
			}

			return false;
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			if (!base.StatefulGraphic.HitTest(mouseInformation.Location))
			{
				base.StatefulGraphic.State = base.StatefulGraphic.CreateInactiveState();
				return false;
			}

			return true;
		}

		public override void OnEnterState(IMouseInformation mouseInformation)
		{
			base.StatefulGraphic.OnEnterSelectedState(mouseInformation);
		}

		public override string ToString()
		{
			return "SelectedGraphicState\n";
		}
	}
}
