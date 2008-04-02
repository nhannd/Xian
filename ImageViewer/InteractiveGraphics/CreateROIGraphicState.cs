#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Drawing;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	internal class CreateRoiGraphicState : CreateGraphicState
	{
		private StatefulCompositeGraphic _childGraphic;

		public CreateRoiGraphicState(RoiGraphic roiGraphic)
			: base(roiGraphic)
		{
		}

		protected RoiGraphic ROIGraphic
		{
			get { return this.StandardStatefulGraphic as RoiGraphic; }
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			if (_childGraphic == null)
			{
				PointF mousePoint = new PointF(mouseInformation.Location.X, mouseInformation.Location.Y);

				this.ROIGraphic.Roi.CoordinateSystem = CoordinateSystem.Destination;

				// Suspend the ROIChanged event while we initialize the control points
				this.ROIGraphic.SuspendRoiChangedEvent();

				for (int i = 0; i < this.ROIGraphic.Roi.ControlPoints.Count; i++)
					this.ROIGraphic.Roi.ControlPoints[i] = mousePoint;

				// Now we're ready to broadcast the ROIChanged event to everyone who's listening
				this.ROIGraphic.ResumeRoiChangedEvent(true);

				this.ROIGraphic.Roi.ResetCoordinateSystem();

				_childGraphic = this.ROIGraphic.Roi;
				_childGraphic.StateChanged += new EventHandler<GraphicStateChangedEventArgs>(OnRoiStateChanged);
			}

			return _childGraphic.Start(mouseInformation);
		}


		public override bool Track(IMouseInformation mouseInformation)
		{
			// Route mouse move message to the child roi object
			if (_childGraphic != null)
				return _childGraphic.Track(mouseInformation);

			return false;
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			// Route mouse move message to the child roi object
			if (_childGraphic != null)
				return _childGraphic.Stop(mouseInformation);

			return false;
		}

		private void OnRoiStateChanged(object sender, GraphicStateChangedEventArgs e)
		{
			// When the child ROI graphic transitions to the focus selected state,
			// it causes "this" to transition to selected state too.
			if (e.NewState is FocussedSelectedGraphicState)
			{
				_childGraphic.StateChanged -= new EventHandler<GraphicStateChangedEventArgs>(OnRoiStateChanged);
				_childGraphic = null;

				this.ROIGraphic.State = this.ROIGraphic.CreateFocussedSelectedState();
			}
		}

		public override string ToString()
		{
			return "CreateROIGraphicState\n";
		}
	}
}
