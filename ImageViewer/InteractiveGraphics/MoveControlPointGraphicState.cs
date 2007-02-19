using System;
using System.Diagnostics;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	public class MoveControlPointGraphicState : StandardGraphicState
	{
		private PointF _currentPoint;
		private PointF _startPoint;
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
			get { return this.StandardStatefulGraphic as InteractiveGraphic; }
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			base.LastPoint = this.InteractiveGraphic.SpatialTransform.ConvertToSource(mouseInformation.Location);
			_startPoint = _currentPoint = base.LastPoint;

			if (this.SupportUndo)
			{
				base.Command = new PositionGraphicCommand(this.InteractiveGraphic);
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
			//only save undo info if the point moved
			if (this.SupportUndo && _startPoint != _currentPoint)
			{
				PositionGraphicCommand cmd = base.Command as PositionGraphicCommand;
				cmd.EndState = this.InteractiveGraphic.CreateMemento();
				this.InteractiveGraphic.ImageViewer.CommandHistory.AddCommand(base.Command);
			}

			this.StandardStatefulGraphic.State = this.StandardStatefulGraphic.CreateFocusSelectedState();
		}

		public override string ToString()
		{
			return "Move Control Point State\n";
		}
	}
}
