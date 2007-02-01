using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using System.Drawing;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	public class SelectedGraphicState : StandardGraphicState
	{
		public SelectedGraphicState(IStandardStatefulGraphic standardStatefulGraphic)
			: base(standardStatefulGraphic)
		{
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			if (this.StandardStatefulGraphic.HitTest(mouseInformation.Location))
			{
				this.StandardStatefulGraphic.State = this.StandardStatefulGraphic.CreateFocusSelectedState();
				return true;
			}

			return false;
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			if (!this.StandardStatefulGraphic.HitTest(mouseInformation.Location))
			{
				this.StandardStatefulGraphic.State = this.StandardStatefulGraphic.CreateInactiveState();
				return false;
			}

			return true;
		}

		public override string ToString()
		{
			return "SelectedGraphicState\n";
		}
	}
}
