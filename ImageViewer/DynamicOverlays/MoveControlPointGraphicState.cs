using System;
using System.Diagnostics;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.DynamicOverlays
{
	public class MoveControlPointGraphicState : GraphicState
	{
		private PointF _currentPoint;
		private int _controlPointIndex;

		// Edit an overlay object control point
		public MoveControlPointGraphicState(
			InteractiveGraphic interactiveGraphic, 
			int controlPointIndex) : base(interactiveGraphic)
		{
			Platform.CheckForNullReference(interactiveGraphic, "interactiveGraphic");
			Platform.CheckNonNegative(controlPointIndex, "controlPointIndex");

			_controlPointIndex = controlPointIndex;
		}

		private InteractiveGraphic InteractiveGraphic
		{
			get { return base.StatefulGraphic as InteractiveGraphic; }
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			base.LastPoint = this.InteractiveGraphic.SpatialTransform.ConvertToSource(mouseInformation.Location);

			if (this.SupportUndo)
			{
				base.Command = new PositionGraphicCommand(this.InteractiveGraphic, false);
				base.Command.Name = SR.CommandMoveControlPoint;
				PositionGraphicCommand cmd = base.Command as PositionGraphicCommand;
				cmd.BeginState = this.InteractiveGraphic.CreateMemento();
			}

			return true;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			if (base.Command == null)
				return false;

			_currentPoint = this.InteractiveGraphic.SpatialTransform.ConvertToSource(mouseInformation.Location);
			_currentPoint = this.InteractiveGraphic.CalcControlPointPosition(_controlPointIndex, base.LastPoint, _currentPoint);
			this.InteractiveGraphic.CoordinateSystem = CoordinateSystem.Source;
			this.InteractiveGraphic.ControlPoints[_controlPointIndex] = _currentPoint;
			this.InteractiveGraphic.ResetCoordinateSystem();
			this.InteractiveGraphic.Draw();

			base.LastPoint = _currentPoint;

			return true;
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			Cancel();
			return false;
		}

		public override void Cancel()
		{
			if (this.SupportUndo)
			{
				PositionGraphicCommand cmd = base.Command as PositionGraphicCommand;
				cmd.EndState = this.InteractiveGraphic.CreateMemento();
				this.InteractiveGraphic.ImageViewer.CommandHistory.AddCommand(base.Command);
			}

			base.StatefulGraphic.State = base.StatefulGraphic.CreateFocusSelectedState();
		}

		public override string ToString()
		{
			return "Move Control Point State\n";
		}
	}
}
