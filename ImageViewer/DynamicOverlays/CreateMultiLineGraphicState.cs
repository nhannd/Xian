using System;
using System.Diagnostics;
using System.Drawing;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.DynamicOverlays
{
	/// <summary>
	/// Summary description for InactiveState.
	/// </summary>
	public class CreateMultiLineGraphicState : GraphicState
	{
		private int m_ControlPointIndex;
		private int m_NumberOfPointsAnchored = 0;

		// Create a graphic object
		public CreateMultiLineGraphicState(InteractiveMultiLineGraphic interactiveGraphic) 
			: base(interactiveGraphic)
		{
			m_ControlPointIndex = 1;
		}

		private InteractiveMultiLineGraphic InteractiveGraphic
		{
			get { return base.StatefulGraphic as InteractiveMultiLineGraphic; }
		}

		public override bool OnMouseDown(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			m_NumberOfPointsAnchored++;

			// We just started creating
			if (m_NumberOfPointsAnchored == 1)
			{
				PointF mousePoint = new PointF(e.X, e.Y);
				this.InteractiveGraphic.CoordinateSystem = CoordinateSystem.Destination;
				this.InteractiveGraphic.ControlPoints[0] = mousePoint;
				this.InteractiveGraphic.ControlPoints[1] = mousePoint;
				this.InteractiveGraphic.ResetCoordinateSystem();
			}
			// We're done creating
			else if (m_NumberOfPointsAnchored == this.InteractiveGraphic.MaximumAnchorPoints)
			{
				if (this.SupportUndo)
				{
					base.Command = new PositionGraphicCommand(this.InteractiveGraphic, true);
					base.Command.Name = SR.CommandCreateMultilineGraphic;
					this.InteractiveGraphic.ParentWorkspace.CommandHistory.AddCommand(base.Command);
				}

				base.StatefulGraphic.State = base.StatefulGraphic.CreateSelectedState();
			}
			// We're in the middle of creating
			else if (m_NumberOfPointsAnchored >= 2 && this.InteractiveGraphic.MaximumAnchorPoints > 2)
			{
				PointF pt = new PointF(e.X, e.Y);

				this.InteractiveGraphic.CoordinateSystem = CoordinateSystem.Destination;
				this.InteractiveGraphic.AnchorPoints.Add(pt);
				this.InteractiveGraphic.ControlPoints.Add(pt);
				this.InteractiveGraphic.ResetCoordinateSystem();

				m_ControlPointIndex = this.InteractiveGraphic.ControlPoints.Count - 1;
			}

			return true;
		}

		public override bool OnMouseMove(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			PointF pt = new PointF(e.X, e.Y);

			this.InteractiveGraphic.CoordinateSystem = CoordinateSystem.Destination;
			this.InteractiveGraphic.ControlPoints[m_ControlPointIndex] = pt;
			this.InteractiveGraphic.ResetCoordinateSystem();
			this.InteractiveGraphic.Draw();

			return true;
		}

		public override string ToString()
		{
			return "CreateMultilineGraphicState\n";
		}
	}
}
