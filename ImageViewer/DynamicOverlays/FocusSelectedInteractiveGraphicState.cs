using System;
using System.Drawing;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.DynamicOverlays
{
	public class FocusSelectedInteractiveGraphicState : FocusSelectedGraphicState
	{
		public FocusSelectedInteractiveGraphicState(InteractiveGraphic interactiveGraphic)
			: base(interactiveGraphic)
		{
		}

		public override bool Start(MouseInformation pointerInformation)
		{
			InteractiveGraphic interactiveGraphic = base.StatefulGraphic as InteractiveGraphic;
			int controlPointIndex = interactiveGraphic.ControlPoints.HitTestControlPoint(pointerInformation.Point);

			// User has clicked a control point
			if (controlPointIndex != -1)
			{
				Trace.Write(String.Format("Control Point {0}\n", controlPointIndex.ToString()));
				base.StatefulGraphic.State = new MoveControlPointGraphicState(interactiveGraphic, controlPointIndex);
				base.StatefulGraphic.State.Start(pointerInformation);
				return true;
			}

			return base.Start(pointerInformation);
		}

		public override string ToString()
		{
			return "FocusSelectedInteractiveGraphicState\n";
		}
	}
}
