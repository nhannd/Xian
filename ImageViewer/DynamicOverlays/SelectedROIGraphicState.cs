using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Layers;

namespace ClearCanvas.ImageViewer.DynamicOverlays
{
	public class SelectedROIGraphicState : GraphicState
	{
		private StatefulGraphic _currentChildGraphic;

		public SelectedROIGraphicState(ROIGraphic roiGraphic)
			: base(roiGraphic)
		{
			_currentChildGraphic = this.ROIGraphic.Roi;
		}

		private ROIGraphic ROIGraphic
		{
			get { return base.StatefulGraphic as ROIGraphic; }
		}

		public override bool OnMouseDown(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			if (this.ROIGraphic.Roi.HitTest(e))
			{
				_currentChildGraphic = this.ROIGraphic.Roi;
				_currentChildGraphic.State = this.ROIGraphic.Roi.CreateSelectedState();
				return _currentChildGraphic.OnMouseDown(e);
			}
			else if (this.ROIGraphic.Callout.HitTest(e))
			{
				_currentChildGraphic = this.ROIGraphic.Callout;
				_currentChildGraphic.State = this.ROIGraphic.Callout.CreateSelectedState();
				return _currentChildGraphic.OnMouseDown(e);
			}

			this.ROIGraphic.State = this.ROIGraphic.CreateInactiveState();

			return false;
		}

		public override bool OnMouseMove(XMouseEventArgs e)
		{
			// In order to indicate which tool will be acted upon,
			// the selected graphic must also be able to be focused.
			if (base.StatefulGraphic.HitTest(e))
			{
				base.StatefulGraphic.State = base.StatefulGraphic.CreateFocusState();
				return true;
			}

			if (_currentChildGraphic != null)
				return _currentChildGraphic.OnMouseMove(e);

			return false;
		}

		public override bool OnMouseUp(XMouseEventArgs e)
		{
			if (_currentChildGraphic != null)
			{
				bool result = _currentChildGraphic.OnMouseUp(e);
				_currentChildGraphic = null;
				return result;
			}

			return false;
		}

		public override void OnEnterState(XMouseEventArgs e)
		{
			base.StatefulGraphic.OnEnterSelectedState(e);
		}

		public override string ToString()
		{
			return "SelectedROIGraphicState\n";
		}
	}
}
