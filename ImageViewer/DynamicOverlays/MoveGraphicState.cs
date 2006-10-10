using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Layers;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.DynamicOverlays
{
	public class MoveGraphicState : GraphicState
	{
		private PointF _currentPoint = new Point(0,0);

		public MoveGraphicState(StatefulGraphic statefulGraphic)
			: base(statefulGraphic)
		{
		}

		public override bool OnMouseDown(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			if (e.MouseCapture != null)
				e.MouseCapture.SetCapture(this, e);

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

			//When moving, graphics are confined to 
			Point mousePoint = new Point(e.X, e.Y);
			PointUtilities.ConfinePointToRectangle(ref mousePoint, this.StatefulGraphic.ParentLayerManager.RootLayerGroup.DestinationRectangle);

			_currentPoint = this.StatefulGraphic.SpatialTransform.ConvertToSource(new PointF(mousePoint.X, mousePoint.Y));

			base.StatefulGraphic.CoordinateSystem = CoordinateSystem.Source;
			SizeF delta = Graphic.CalcGraphicPositionDelta(base.LastPoint, _currentPoint);
			base.StatefulGraphic.Move(delta);
			base.StatefulGraphic.ResetCoordinateSystem();
			base.StatefulGraphic.Draw();

			base.LastPoint = _currentPoint;

			return true;
		}

		public override bool OnMouseUp(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			if (e.MouseCapture != null)
				e.MouseCapture.ReleaseCapture();

			if (this.SupportUndo)
			{
				base.Command.EndState = (base.StatefulGraphic as IMemorable).CreateMemento();
				this.StatefulGraphic.ImageViewer.CommandHistory.AddCommand(base.Command);
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
