using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Layers;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.DynamicOverlays
{
	public class ROIGraphic : StatefulGraphic, IMemorable, IMouseButtonHandler
    {
        private InteractiveGraphic _roiGraphic;
        private CalloutGraphic _calloutGraphic;
		private event EventHandler _roiChangedEvent;

        public ROIGraphic(InteractiveGraphic graphic, bool userCreated)
        {
			_roiGraphic = graphic;

			BuildGraphic();

			if (userCreated)
				base.State = CreateCreateState();
			else
				base.State = CreateInactiveState();
		}

		public CalloutGraphic Callout
		{
			get { return _calloutGraphic; }
		}

        public InteractiveGraphic Roi
        {
            get { return _roiGraphic; }
        }

		public event EventHandler RoiChanged
		{
			add { _roiChangedEvent += value; }
			remove { _roiChangedEvent -= value; }
		}

		public override GraphicState CreateCreateState()
		{
			return new CreateROIGraphicState(this);
		}

		public override GraphicState CreateInactiveState()
		{
			return new InactiveGraphicState(this);
		}

		public override GraphicState CreateFocusState()
		{
			return new FocusGraphicState(this);
		}

		public override GraphicState CreateFocusSelectedState()
		{
			return new FocusSelectedROIGraphicState(this);
		}

		public override void OnEnterInactiveState(MouseInformation pointerInformation)
		{
			// If the currently selected graphic is this one,
			// and we're about to go inactive, set the selected graphic
			// to null, indicating that no graphic is currently selected
			if (this.ParentLayerManager.SelectedGraphic == this)
				this.ParentLayerManager.SelectedGraphic = null;

			if (this.ParentLayerManager.FocusGraphic == this)
				this.ParentLayerManager.FocusGraphic = null;

			this.Roi.ControlPoints.Visible = false;
			this.Color = Color.Yellow;
			Draw();

			SetCursorToken(pointerInformation);

			Trace.Write("OnEnterInactiveState\n");
		}

		public override void OnEnterFocusState(MouseInformation pointerInformation)
		{
			this.Focused = true;
			
			if (this.Roi.HitTest(pointerInformation.Point))
				this.Roi.ControlPoints.Visible = true;
			else
				this.Roi.ControlPoints.Visible = false;

			this.Color = Color.Orange;
			Draw();

			SetCursorToken(pointerInformation);

			Trace.Write("OnEnterFocusState\n");
		}

		public override void OnEnterFocusSelectedState(MouseInformation pointerInformation)
		{
			this.Selected = true;
			this.Focused = true;

			this.Roi.ControlPoints.Visible = true;
			this.Color = Color.Red;
			Draw();

			SetCursorToken(pointerInformation);

			Trace.Write("OnEnterSelectedState\n");
		}

		public override void OnEnterSelectedState(MouseInformation pointerInformation)
		{
			if (this.ParentLayerManager.FocusGraphic == this)
				this.ParentLayerManager.FocusGraphic = null;

			this.Roi.ControlPoints.Visible = true;
			this.Color = Color.Red;
			Draw();

			SetCursorToken(pointerInformation);

			Trace.Write("OnEnterSelectedState\n");
		}

		public override bool SetCursorToken(MouseInformation pointerInformation)
		{
			if (this.Roi.SetCursorToken(pointerInformation))
				return true;
			
			if (this.Callout.SetCursorToken(pointerInformation))
				return true;

			pointerInformation.Tile.CursorToken = null;
			return false;
		}

		public override bool HitTest(Point point)
		{
			return _roiGraphic.HitTest(point) || _calloutGraphic.HitTest(point);
		}

		#region IMemorable Members

		public IMemento CreateMemento()
		{
			return null;
		}

		public void SetMemento(IMemento memento)
		{
		}

		#endregion

		private void BuildGraphic()
		{
			base.Graphics.Add(_roiGraphic);

			_calloutGraphic = new CalloutGraphic();
			base.Graphics.Add(_calloutGraphic);

			_roiGraphic.ControlPoints.ControlPointChangedEvent += new EventHandler<ControlPointEventArgs>(OnControlPointChanged);
			_calloutGraphic.LocationChanged += new EventHandler<PointChangedEventArgs>(OnCalloutTopLeftChanged);
		}

		private void OnControlPointChanged(object sender, ControlPointEventArgs e)
		{
			// We're attaching the callout to the ROI, so make sure the two
			// graphics are in the same coordinate system before we do that.
			_calloutGraphic.CoordinateSystem = _roiGraphic.CoordinateSystem;
			_calloutGraphic.End = FindClosestControlPoint();
			_calloutGraphic.ResetCoordinateSystem();
			EventsHelper.Fire(_roiChangedEvent, this, EventArgs.Empty);
		}

		private void OnCalloutTopLeftChanged(object sender, PointChangedEventArgs e)
		{
			_calloutGraphic.CoordinateSystem = _roiGraphic.CoordinateSystem;
			_calloutGraphic.End = FindClosestControlPoint();
			_calloutGraphic.ResetCoordinateSystem();
		}

		private PointF FindClosestControlPoint()
		{
			double distance;
			double shortest;
			PointF closestPoint;
			PointF calloutStart = _calloutGraphic.Start;

			shortest = double.MaxValue;
			closestPoint = new PointF(float.MaxValue, float.MaxValue);

			for (int i = 0; i < _roiGraphic.ControlPoints.Count; i++)
			{
				PointF controlPoint = _roiGraphic.ControlPoints[i];
				distance = Vector.Distance(controlPoint, calloutStart);

				if (distance < shortest)
				{
					shortest = distance;
					closestPoint = controlPoint;
				}
			}

			return closestPoint;
		}
	}
}