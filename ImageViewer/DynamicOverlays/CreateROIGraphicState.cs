using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ClearCanvas.Workstation.Model.Layers;
using ClearCanvas.Common;

namespace ClearCanvas.Workstation.Model.DynamicOverlays
{
	public class CreateROIGraphicState : GraphicState
	{
		private StatefulGraphic _childGraphic;

		public CreateROIGraphicState(ROIGraphic roiGraphic)
			: base(roiGraphic)
		{
		}

		private ROIGraphic ROIGraphic
		{
			get { return base.StatefulGraphic as ROIGraphic; }
		}

		public override bool OnMouseDown(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			if (_childGraphic == null)
			{
				// Create the callout
				PointF mousePoint = new PointF(e.X, e.Y);

#if MONO
				Size offset = new Size(50, 30);
#else
				SizeF offset = new SizeF(50, 30);
#endif

				PointF calloutLocation = mousePoint - offset;
				this.ROIGraphic.Callout.CoordinateSystem = CoordinateSystem.Destination;
				this.ROIGraphic.Callout.Location = calloutLocation;
				this.ROIGraphic.Callout.End = mousePoint;
				this.ROIGraphic.Callout.ResetCoordinateSystem();

				this.ROIGraphic.Roi.CoordinateSystem = CoordinateSystem.Destination;

				for (int i = 0; i < this.ROIGraphic.Roi.ControlPoints.Count; i++ )
					this.ROIGraphic.Roi.ControlPoints[i] = mousePoint;

				this.ROIGraphic.Roi.ResetCoordinateSystem();

				this.ROIGraphic.Callout.Draw();

				// We want to route mouse messages to this.ROIGraphic.Roi so that
				// the creation of the ROI is delegated to that object
				_childGraphic = this.ROIGraphic.Roi;
				_childGraphic.State.SupportUndo = false;
				_childGraphic.StateChanged += new EventHandler<GraphicStateChangedEventArgs>(OnRoiStateChanged);
			}

			// Actually route the mouse down message
			return _childGraphic.OnMouseDown(e);
		}

		public override bool OnMouseMove(XMouseEventArgs e)
		{
			// Route mouse move message to the child roi object
			if (_childGraphic != null)
				return _childGraphic.OnMouseMove(e);

			return false;
		}

		private void OnRoiStateChanged(object sender, GraphicStateChangedEventArgs e)
		{
			// When the child ROI graphic transitions to the selected state,
			// cause "this" to transition to selected state too.
			if (e.NewState is SelectedGraphicState)
			{
				this.ROIGraphic.State = this.ROIGraphic.CreateSelectedState();
				_childGraphic.StateChanged -= new EventHandler<GraphicStateChangedEventArgs>(OnRoiStateChanged);
				_childGraphic = null;

				// We're done creating, so create a command
				this.Command = new PositionGraphicCommand(this.ROIGraphic, true);
				this.Command.Name = SR.CommandCreateROIGraphic;
				this.ROIGraphic.ParentWorkspace.CommandHistory.AddCommand(base.Command);
			}
		}

		public override string ToString()
		{
			return "CreateROIGraphicState\n";
		}
	}
}
