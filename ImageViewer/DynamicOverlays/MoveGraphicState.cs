using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Layers;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.DynamicOverlays
{
	public class MoveGraphicState : GraphicState
	{
		private PointF _currentPoint = new Point(0,0);

		public MoveGraphicState(StatefulGraphic statefulGraphic)
			: base(statefulGraphic)
		{
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			base.LastPoint = this.StatefulGraphic.SpatialTransform.ConvertToSource(mouseInformation.Location);

			if (this.SupportUndo)
			{
				base.Command = new PositionGraphicCommand(base.StatefulGraphic, false);
				base.Command.Name = SR.CommandMoveGraphic;
				base.Command.BeginState = (base.StatefulGraphic as IMemorable).CreateMemento();
			}

			return true;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			_currentPoint = this.StatefulGraphic.SpatialTransform.ConvertToSource(mouseInformation.Location);

			base.StatefulGraphic.CoordinateSystem = CoordinateSystem.Source;
			SizeF delta = Graphic.CalcGraphicPositionDelta(base.LastPoint, _currentPoint);
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
			if (this.SupportUndo)
			{
				base.Command.EndState = (base.StatefulGraphic as IMemorable).CreateMemento();
				this.StatefulGraphic.ImageViewer.CommandHistory.AddCommand(base.Command);
			}

			base.StatefulGraphic.State = base.StatefulGraphic.CreateFocusSelectedState();
		}

		public override string ToString()
		{
			return "Move State\n";
		}
	}
}
