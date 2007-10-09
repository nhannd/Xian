using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	public class FocussedSelectedRoiGraphicState : FocussedSelectedGraphicState
	{
		private StatefulCompositeGraphic _currentChildGraphic;

		public FocussedSelectedRoiGraphicState(RoiGraphic roiGraphic)
			: base(roiGraphic)
		{
			_currentChildGraphic = null;
		}

		protected RoiGraphic ROIGraphic
		{
			get { return this.StandardStatefulGraphic as RoiGraphic; }
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

			this.StandardStatefulGraphic.State = this.StandardStatefulGraphic.CreateSelectedState();
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

		public override string ToString()
		{
			return "FocusSelectedROIGraphicState\n";
		}
	}
}
