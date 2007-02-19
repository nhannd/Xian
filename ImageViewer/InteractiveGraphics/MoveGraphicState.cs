using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	public class MoveGraphicState : StandardGraphicState
	{
		private PointF _currentPoint = new Point(0,0);
		private PointF _startPoint;

		public MoveGraphicState(IStandardStatefulGraphic standardStatefulGraphic)
			: base(standardStatefulGraphic)
		{
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			base.LastPoint = this.StandardStatefulGraphic.SpatialTransform.ConvertToSource(mouseInformation.Location);
			_startPoint = _currentPoint = base.LastPoint;

			if (this.SupportUndo)
			{
				base.Command = new PositionGraphicCommand(base.StatefulGraphic);
				base.Command.Name = SR.CommandMoveGraphic;
				base.Command.BeginState = (base.StatefulGraphic as IMemorable).CreateMemento();
			}

			return true;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			_currentPoint = this.StandardStatefulGraphic.SpatialTransform.ConvertToSource(mouseInformation.Location);

			base.StatefulGraphic.CoordinateSystem = CoordinateSystem.Source;
			SizeF delta = VectorGraphic.CalcGraphicPositionDelta(base.LastPoint, _currentPoint);
			base.StatefulGraphic.Move(delta);
			base.StatefulGraphic.ResetCoordinateSystem();
			base.StatefulGraphic.Draw();

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
			if (this.SupportUndo && _startPoint != _currentPoint)
			{
				base.Command.EndState = (base.StatefulGraphic as IMemorable).CreateMemento();
				this.StandardStatefulGraphic.ImageViewer.CommandHistory.AddCommand(base.Command);
			}

			this.StandardStatefulGraphic.State = this.StandardStatefulGraphic.CreateFocusSelectedState();
		}

		public override string ToString()
		{
			return "Move State\n";
		}
	}
}
