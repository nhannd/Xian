using System;
using System.Diagnostics;
using System.Drawing;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.DynamicOverlays
{
	/// <summary>
	/// Summary description for InactiveState.
	/// </summary>
	public class MoveControlPointGraphicState : GraphicState
	{
		private PointF m_CurrentPoint;
		private int m_ControlPointIndex;

		// Edit an overlay object control point
		public MoveControlPointGraphicState(
			InteractiveGraphic interactiveGraphic, 
			int controlPointIndex) : base(interactiveGraphic)
		{
			Platform.CheckForNullReference(interactiveGraphic, "interactiveGraphic");
			Platform.CheckNonNegative(controlPointIndex, "controlPointIndex");

			m_ControlPointIndex = controlPointIndex;
		}

		private InteractiveGraphic InteractiveGraphic
		{
			get { return base.StatefulGraphic as InteractiveGraphic; }
		}

		public override bool OnMouseDown(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			base.LastPoint = this.InteractiveGraphic.SpatialTransform.ConvertToSource(new PointF(e.X, e.Y));

			if (this.SupportUndo)
			{
				base.Command = new PositionGraphicCommand(this.InteractiveGraphic, false);
				base.Command.Name = SR.CommandMoveControlPoint;
				PositionGraphicCommand cmd = base.Command as PositionGraphicCommand;
				cmd.BeginState = this.InteractiveGraphic.CreateMemento();
			}

			return true;
		}

		public override bool OnMouseMove(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			if (base.Command == null)
				return false;

			m_CurrentPoint = this.InteractiveGraphic.SpatialTransform.ConvertToSource(new PointF(e.X, e.Y));
			m_CurrentPoint = this.InteractiveGraphic.CalcControlPointPosition(m_ControlPointIndex, base.LastPoint, m_CurrentPoint);
			this.InteractiveGraphic.CoordinateSystem = CoordinateSystem.Source;
			this.InteractiveGraphic.ControlPoints[m_ControlPointIndex] = m_CurrentPoint;
			this.InteractiveGraphic.ResetCoordinateSystem();
			this.InteractiveGraphic.Draw();

			base.LastPoint = m_CurrentPoint;

			return true;
		}

		public override bool OnMouseUp(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			if (this.SupportUndo)
			{
				PositionGraphicCommand cmd = base.Command as PositionGraphicCommand;
				cmd.EndState = this.InteractiveGraphic.CreateMemento();
				this.InteractiveGraphic.ParentWorkspace.CommandHistory.AddCommand(base.Command);
			}

			base.StatefulGraphic.State = base.StatefulGraphic.CreateSelectedState();

			return true;
		}

		public override string ToString()
		{
			return "Move Control Point State\n";
		}
	}
}
