using System;
using System.Drawing;
using System.Diagnostics;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.DynamicOverlays
{
	public class SelectedInteractiveGraphicState : SelectedGraphicState
	{
		public SelectedInteractiveGraphicState(InteractiveGraphic interactiveGraphic)
			: base(interactiveGraphic)
		{
		}

		public override bool OnMouseDown(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			InteractiveGraphic interactiveGraphic = base.StatefulGraphic as InteractiveGraphic;
			int controlPointIndex = interactiveGraphic.ControlPoints.HitTestControlPoint(e);

			// User has clicked a control point
			if (controlPointIndex != -1)
			{
				Trace.Write(String.Format("Control Point {0}\n", controlPointIndex.ToString()));
				base.StatefulGraphic.State = new MoveControlPointGraphicState(interactiveGraphic, controlPointIndex);
				base.StatefulGraphic.State.OnMouseDown(e);
				return true;
			}

			return base.OnMouseDown(e);
		}

		public override bool OnMouseUp(XMouseEventArgs e)
		{
			// Must return true.  If we just go with the base class
			// implementation which returns false, PresentationImage.OnMouseUp
			// will think we haven't handled this and then route the MouseUp message
			// to some other mouse tool, which we don't want.
			return true;
		}

		public override string ToString()
		{
			return "SelectedInteractiveGraphicState\n";
		}
	}
}
