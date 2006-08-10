using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Layers;

namespace ClearCanvas.ImageViewer.DynamicOverlays
{
	/// <summary>
	/// Summary description for InactiveState.
	/// </summary>
	public class MoveGraphicState : GraphicState
	{
		private PointF _CurrentPoint = new Point(0,0);

		public MoveGraphicState(StatefulGraphic statefulGraphic)
			: base(statefulGraphic)
		{
		}

		public override bool OnMouseDown(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			base.LastPoint = this.StatefulGraphic.SpatialTransform.ConvertToSource(new PointF(e.X, e.Y));

			if (this.SupportUndo)
			{
				base.Command = new PositionGraphicCommand(base.StatefulGraphic, false);
				base.Command.Name = SR.CommandMoveGraphic;
				base.Command.BeginState = (base.StatefulGraphic as IMemorable).CreateMemento();
			}

			return true;
		}

		public override bool OnMouseMove(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			_CurrentPoint = this.StatefulGraphic.SpatialTransform.ConvertToSource(new PointF(e.X, e.Y));

			base.StatefulGraphic.CoordinateSystem = CoordinateSystem.Source;
			SizeF delta = Graphic.CalcGraphicPositionDelta(base.LastPoint, _CurrentPoint);
			base.StatefulGraphic.Move(delta);
			base.StatefulGraphic.ResetCoordinateSystem();
			base.StatefulGraphic.Draw();

			base.LastPoint = _CurrentPoint;

			return true;
		}

		public override bool OnMouseUp(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			if (this.SupportUndo)
			{
				base.Command.EndState = (base.StatefulGraphic as IMemorable).CreateMemento();
				this.StatefulGraphic.ParentWorkspace.CommandHistory.AddCommand(base.Command);
			}

			base.StatefulGraphic.State = base.StatefulGraphic.CreateSelectedState();

			return true;
		}

		public override string ToString()
		{
			return "Move State\n";
		}
	}
}
