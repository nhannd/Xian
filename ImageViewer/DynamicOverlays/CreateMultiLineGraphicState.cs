using System;
using System.Diagnostics;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.DynamicOverlays
{
	public class CreateMultiLineGraphicState : CreateGraphicState
	{
		private int _controlPointIndex;
		private int _numberOfPointsAnchored = 0;

		// Create a graphic object
		public CreateMultiLineGraphicState(InteractiveMultiLineGraphic interactiveGraphic) 
			: base(interactiveGraphic)
		{
			_controlPointIndex = 1;
		}

		private InteractiveMultiLineGraphic InteractiveGraphic
		{
			get { return base.StatefulGraphic as InteractiveMultiLineGraphic; }
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			_numberOfPointsAnchored++;

			// We just started creating
			if (_numberOfPointsAnchored == 1)
			{
				PointF mousePoint = new PointF(mouseInformation.Location.X, mouseInformation.Location.Y);
				this.InteractiveGraphic.CoordinateSystem = CoordinateSystem.Destination;
				this.InteractiveGraphic.ControlPoints[0] = mousePoint;
				this.InteractiveGraphic.ControlPoints[1] = mousePoint;
				this.InteractiveGraphic.ResetCoordinateSystem();
			}
			// We're done creating
			else if (_numberOfPointsAnchored == this.InteractiveGraphic.MaximumAnchorPoints)
			{
				if (this.SupportUndo)
				{
					base.Command = new PositionGraphicCommand(this.InteractiveGraphic, true);
					base.Command.Name = SR.CommandCreateMultilineGraphic;
					this.InteractiveGraphic.ImageViewer.CommandHistory.AddCommand(base.Command);
				}

				base.StatefulGraphic.State = base.StatefulGraphic.CreateFocusSelectedState();
			}
			// We're in the middle of creating
			else if (_numberOfPointsAnchored >= 2 && this.InteractiveGraphic.MaximumAnchorPoints > 2)
			{
				PointF mousePoint = new PointF(mouseInformation.Location.X, mouseInformation.Location.Y);

				this.InteractiveGraphic.CoordinateSystem = CoordinateSystem.Destination;
				this.InteractiveGraphic.AnchorPoints.Add(mousePoint);
				this.InteractiveGraphic.ControlPoints.Add(mousePoint);
				this.InteractiveGraphic.ResetCoordinateSystem();

				_controlPointIndex = this.InteractiveGraphic.ControlPoints.Count - 1;
			}

			return true;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			PointF pt = new PointF(mouseInformation.Location.X, mouseInformation.Location.Y);

			this.InteractiveGraphic.CoordinateSystem = CoordinateSystem.Destination;
			this.InteractiveGraphic.ControlPoints[_controlPointIndex] = pt;
			this.InteractiveGraphic.ResetCoordinateSystem();
			this.InteractiveGraphic.Draw();

			return true;
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			return true;
		}

		public override string ToString()
		{
			return "CreateMultilineGraphicState\n";
		}
	}
}
