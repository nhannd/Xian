using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ClearCanvas.ImageViewer.Layers;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.DynamicOverlays
{
	public class CreateROIGraphicState : GraphicState
	{
		private StatefulGraphic _childGraphic;
		private IMouseCapture _mouseCapture; //objects with atomic mouse capture (up-down up-down, etc) should store this interface and use it.
		private bool _deleteROIGraphic = false;

		public CreateROIGraphicState(ROIGraphic roiGraphic)
			: base(roiGraphic)
		{
		}

		private ROIGraphic ROIGraphic
		{
			get { return base.StatefulGraphic as ROIGraphic; }
		}

		private void ReleaseCapture()
		{
			//release capture when not in the create state anymore.
			if (_mouseCapture != null)
			{
				//we don't need to know about capture changing anymore.
				_mouseCapture.NotifyCaptureChanging -= CaptureChanging;
				
				if (_mouseCapture.GetCapture() == this)
					_mouseCapture.ReleaseCapture();

				_mouseCapture = null;
			}

		}

		private void DeleteROIGraphic()
		{
			ReleaseCapture();
			this.ROIGraphic.ParentLayerManager.SelectedGraphicLayer.Graphics.Remove(this.ROIGraphic);
		}

		private void CaptureChanging(object sender, MouseCaptureChangingEventArgs e)
		{
			//Somebody else has released the capture unexpectedly.
			DeleteROIGraphic();
		}

		public override bool OnMouseDown(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			//set capture on first mouse down.
			if (_mouseCapture == null && e.MouseCapture != null)
			{
				_mouseCapture = e.MouseCapture;
				_mouseCapture.SetCapture(this, e);
				_mouseCapture.NotifyCaptureChanging += this.CaptureChanging;
			}

			if (_mouseCapture != null)
			{
				Point mousePoint = new Point(e.X, e.Y);
				if (!this.StatefulGraphic.SpatialTransform.DestinationRectangle.Contains(mousePoint))
					_deleteROIGraphic = true;
			}

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

		public override bool OnMouseUp(XMouseEventArgs e)
		{
			if (_deleteROIGraphic)
				DeleteROIGraphic();
			else if (this.ROIGraphic.State != this)
				ReleaseCapture();

			return false;
		}
		public override bool OnMouseMove(XMouseEventArgs e)
		{
			Point mousePoint = new Point(e.X, e.Y);
			PointUtilities.ConfinePointToRectangle(ref mousePoint, this.StatefulGraphic.SpatialTransform.DestinationRectangle);
			XMouseEventArgs newMouseEventArgs = new XMouseEventArgs(e.Button, e.Clicks, mousePoint.X, mousePoint.Y, e.Delta, e.ModifierKeys);
			
			// Route mouse move message to the child roi object
			if (_childGraphic != null)
				return _childGraphic.OnMouseMove(newMouseEventArgs);

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
				this.ROIGraphic.ParentViewer.CommandHistory.AddCommand(base.Command);
			}
		}

		public override string ToString()
		{
			return "CreateROIGraphicState\n";
		}
	}
}
