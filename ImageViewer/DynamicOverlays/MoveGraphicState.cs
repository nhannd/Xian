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
	public class MoveGraphicState : GraphicState, IMouseButtonHandler
	{
		private PointF _currentPoint = new Point(0,0);

		public MoveGraphicState(StatefulGraphic statefulGraphic)
			: base(statefulGraphic)
		{
		}

		public override bool Start(MouseInformation pointerInformation)
		{
			pointerInformation.Tile.CurrentPointerAction = this;

			base.LastPoint = this.StatefulGraphic.SpatialTransform.ConvertToSource(pointerInformation.Point);

			if (this.SupportUndo)
			{
				base.Command = new PositionGraphicCommand(base.StatefulGraphic, false);
				base.Command.Name = SR.CommandMoveGraphic;
				base.Command.BeginState = (base.StatefulGraphic as IMemorable).CreateMemento();
			}

			return true;
		}

		public override bool Track(MouseInformation pointerInformation)
		{
			//Point mousePoint = new Point(e.X, e.Y);
			//PointUtilities.ConfinePointToRectangle(ref mousePoint, this.StatefulGraphic.ParentLayerManager.RootLayerGroup.DestinationRectangle);

			_currentPoint = this.StatefulGraphic.SpatialTransform.ConvertToSource(pointerInformation.Point);

			base.StatefulGraphic.CoordinateSystem = CoordinateSystem.Source;
			SizeF delta = Graphic.CalcGraphicPositionDelta(base.LastPoint, _currentPoint);
			base.StatefulGraphic.Move(delta);
			base.StatefulGraphic.ResetCoordinateSystem();
			base.StatefulGraphic.Draw();

			base.LastPoint = _currentPoint;

			return true;
		}

		public override bool Stop(MouseInformation pointerInformation)
		{
			pointerInformation.Tile.CurrentPointerAction = null;

			if (this.SupportUndo)
			{
				base.Command.EndState = (base.StatefulGraphic as IMemorable).CreateMemento();
				this.StatefulGraphic.ImageViewer.CommandHistory.AddCommand(base.Command);
			}

			base.StatefulGraphic.State = base.StatefulGraphic.CreateFocusSelectedState();

			return true;
		}

		public override string ToString()
		{
			return "Move State\n";
		}
	}
}
