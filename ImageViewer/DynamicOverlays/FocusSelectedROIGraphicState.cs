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
			SetGraphicStates(true);
		}

		private ROIGraphic ROIGraphic
		{
			get { return base.StatefulGraphic as ROIGraphic; }
		}

		private void SetGraphicStates(bool focus)
		{
			if (focus)
			{
				this.ROIGraphic.Roi.State = this.ROIGraphic.Roi.CreateFocusSelectedState();
				this.ROIGraphic.Callout.State = this.ROIGraphic.Callout.CreateFocusSelectedState();
			}
			else
			{
				this.ROIGraphic.Roi.State = this.ROIGraphic.Roi.CreateSelectedState();
				this.ROIGraphic.Callout.State = this.ROIGraphic.Callout.CreateSelectedState();

				base.StatefulGraphic.State = base.StatefulGraphic.CreateSelectedState();
			}
		}

		private void SetCurrentChildGraphic(Point point)
		{
			if (this.ROIGraphic.Roi.HitTest(point))
			{
				_currentChildGraphic = this.ROIGraphic.Roi;
			}
			else if (this.ROIGraphic.Callout.HitTest(point))
			{
				_currentChildGraphic = this.ROIGraphic.Callout;
			}
			else
			{
				_currentChildGraphic = null;
			}
		}

		public override bool Start(MouseInformation pointerInformation)
		{
			SetCurrentChildGraphic(pointerInformation.Point);

			if (_currentChildGraphic != null)
				return _currentChildGraphic.Start(pointerInformation);

			//We should never actually get to here, but if we did, this should happen.
			SetGraphicStates(false);
			return false;
		}

		public override bool Track(MouseInformation pointerInformation)
		{
			SetCurrentChildGraphic(pointerInformation.Point);

			if (_currentChildGraphic != null)
				return _currentChildGraphic.Track(pointerInformation);

			SetGraphicStates(false);
			return false;
		}

		public override void OnEnterState(MouseInformation pointerInformation)
		{
			base.StatefulGraphic.OnEnterFocusSelectedState(pointerInformation);
		}

		public override string ToString()
		{
			return "FocusSelectedROIGraphicState\n";
		}
	}
}
