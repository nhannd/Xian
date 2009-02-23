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

using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	public class CreateBoundableGraphicState : CreateInteractiveGraphicState
	{
		private int _controlPointIndex;
		private int _numberOfPointsAnchored = 1;

		// Create a graphic object
		public CreateBoundableGraphicState(IStandardStatefulInteractiveGraphic standardStatefulInteractiveGraphic)
			: base(standardStatefulInteractiveGraphic)
		{
			// This control point index corresponds to the bottom right control point
			_controlPointIndex = 3;
		}

		protected BoundableInteractiveGraphic InteractiveGraphic
		{
			get { return ((IStandardStatefulInteractiveGraphic)base.StatefulGraphic).InteractiveGraphic as BoundableInteractiveGraphic; }
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			// We just started creating
			if (_numberOfPointsAnchored == 1)
			{
				this.InteractiveGraphic.CoordinateSystem = CoordinateSystem.Destination;
				this.InteractiveGraphic.ControlPoints[0] = mouseInformation.Location;
				this.InteractiveGraphic.ControlPoints[3] = mouseInformation.Location;
				this.InteractiveGraphic.ResetCoordinateSystem();

				_numberOfPointsAnchored++;
			}
				// We're done creating
			else
			{
				this.StatefulGraphic.State = this.StatefulGraphic.CreateFocussedSelectedState();
			}

			return true;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			this.InteractiveGraphic.CoordinateSystem = CoordinateSystem.Destination;
			this.InteractiveGraphic.ControlPoints[_controlPointIndex] = mouseInformation.Location;
			this.InteractiveGraphic.ResetCoordinateSystem();
			this.InteractiveGraphic.Draw();

			return true;
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			return true;
		}
	}
}