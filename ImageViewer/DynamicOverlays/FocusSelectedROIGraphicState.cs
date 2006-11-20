using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Layers;
using System.Drawing;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.DynamicOverlays
{
	public class FocusSelectedROIGraphicState : GraphicState
	{
		private StatefulGraphic _currentChildGraphic;

		public FocusSelectedROIGraphicState(ROIGraphic roiGraphic)
			: base(roiGraphic)
		{
			_currentChildGraphic = null;
		}

		private ROIGraphic ROIGraphic
		{
			get { return base.StatefulGraphic as ROIGraphic; }
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			_currentChildGraphic = this.ROIGraphic.Roi;
			if (_currentChildGraphic.Start(mouseInformation))
				return true;

			_currentChildGraphic = this.ROIGraphic.Callout;
			if(_currentChildGraphic.Start(mouseInformation))
				return true;

			_currentChildGraphic = null;
			return false;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			if (_currentChildGraphic != null)
				return _currentChildGraphic.Track(mouseInformation);

			bool returnValue = this.ROIGraphic.Roi.Track(mouseInformation);
			returnValue |= this.ROIGraphic.Callout.Track(mouseInformation);

			if (returnValue)
				return true;

			this.StatefulGraphic.State = this.StatefulGraphic.CreateSelectedState();
			return false;
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			bool returnValue = false;

			if (_currentChildGraphic != null)
				returnValue = _currentChildGraphic.Stop(mouseInformation);

			if (!returnValue)
				_currentChildGraphic = null;

			return returnValue;
		}

		public override void Cancel()
		{
			if (_currentChildGraphic != null)
				_currentChildGraphic.Cancel();

			_currentChildGraphic = null;
		}

		public override void OnEnterState(IMouseInformation mouseInformation)
		{
			base.StatefulGraphic.OnEnterFocusSelectedState(mouseInformation);
		}

		public override string ToString()
		{
			return "FocusSelectedROIGraphicState\n";
		}
	}
}
