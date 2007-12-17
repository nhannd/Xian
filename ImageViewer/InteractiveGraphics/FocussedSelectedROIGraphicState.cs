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

using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	internal class FocussedSelectedRoiGraphicState : FocussedSelectedGraphicState
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
