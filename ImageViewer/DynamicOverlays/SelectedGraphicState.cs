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

		public override bool Track(MouseInformation pointerInformation)
		{
			if (base.StatefulGraphic.HitTest(pointerInformation.Point))
			{
				base.StatefulGraphic.State = base.StatefulGraphic.CreateFocusSelectedState();
				return true;
			}

			return false;
		}

		public override bool Start(MouseInformation pointerInformation)
		{
			if (!base.StatefulGraphic.HitTest(pointerInformation.Point))
			{
				base.StatefulGraphic.State = base.StatefulGraphic.CreateInactiveState();
				return false;
			}

			return true;
		}

		public override void OnEnterState(MouseInformation pointerInformation)
		{
			base.StatefulGraphic.OnEnterSelectedState(pointerInformation);
		}

		public override string ToString()
		{
			return "SelectedGraphicState\n";
		}
	}
}
