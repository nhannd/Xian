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

using System.Drawing;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	internal class CreatePolygonGraphicState : CreateGraphicState
	{
		private const float SNAP_RADIUS = 7.5f;
		private int _controlPointIndex;
		private int _numberOfPointsAnchored = 0;

		// Create a graphic object
		public CreatePolygonGraphicState(PolygonInteractiveGraphic interactiveGraphic) : base(interactiveGraphic)
		{
			_controlPointIndex = 1;
		}

		private PolygonInteractiveGraphic InteractiveGraphic
		{
			get { return this.StandardStatefulGraphic as PolygonInteractiveGraphic; }
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			_numberOfPointsAnchored++;

			// We just started creating
			if (_numberOfPointsAnchored == 1)
			{
				this.InteractiveGraphic.CoordinateSystem = CoordinateSystem.Destination;
				this.InteractiveGraphic.ControlPoints[0] = mouseInformation.Location;
				this.InteractiveGraphic.ControlPoints[1] = mouseInformation.Location;
				this.InteractiveGraphic.ResetCoordinateSystem();
			}
			// We're done creating
			else if (_numberOfPointsAnchored >= 5 && mouseInformation.ClickCount >= 2)
			{
				this.InteractiveGraphic.CoordinateSystem = CoordinateSystem.Destination;

				try {
					_numberOfPointsAnchored--;
					this.InteractiveGraphic.PolyLine.RemoveAt(_numberOfPointsAnchored);
					this.InteractiveGraphic.ControlPoints.RemoveAt(_controlPointIndex--);
					this.InteractiveGraphic.ControlPoints.RemoveAt(_controlPointIndex--);
					this.InteractiveGraphic.ClosePolygon();
				}
				finally
				{
					this.InteractiveGraphic.ResetCoordinateSystem();
				}

				this.StandardStatefulGraphic.State = this.StandardStatefulGraphic.CreateFocussedSelectedState();
			}
			// We're done creating
			else if (_numberOfPointsAnchored >= 3 && AtOrigin(mouseInformation.Location) && mouseInformation.ClickCount == 1)
			{
				this.InteractiveGraphic.CoordinateSystem = CoordinateSystem.Destination;

				try {
					this.InteractiveGraphic.ControlPoints.RemoveAt(_controlPointIndex--);
					this.InteractiveGraphic.ClosePolygon();
				} finally {
					this.InteractiveGraphic.ResetCoordinateSystem();
				}

				this.StandardStatefulGraphic.State = this.StandardStatefulGraphic.CreateFocussedSelectedState();
			}
			// We're in the middle of creating
			else if (_numberOfPointsAnchored >= 2 && this.InteractiveGraphic.MaximumAnchorPoints > 2 && mouseInformation.ClickCount == 1)
			{
				this.InteractiveGraphic.CoordinateSystem = CoordinateSystem.Destination;
				this.InteractiveGraphic.PolyLine.Add(mouseInformation.Location);
				this.InteractiveGraphic.ControlPoints.Add(mouseInformation.Location);
				this.InteractiveGraphic.ResetCoordinateSystem();

				_controlPointIndex = this.InteractiveGraphic.ControlPoints.Count - 1;
			}
			else if (mouseInformation.ClickCount > 1)
			{
				_numberOfPointsAnchored--;
			}

			return true;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			this.InteractiveGraphic.CoordinateSystem = CoordinateSystem.Destination;

			if (_numberOfPointsAnchored >= 3 && AtOrigin(mouseInformation.Location)) {
				this.InteractiveGraphic.ControlPoints[_controlPointIndex] = this.InteractiveGraphic.PolyLine[0];
			} else {
				this.InteractiveGraphic.ControlPoints[_controlPointIndex] = mouseInformation.Location;
			}

			this.InteractiveGraphic.ResetCoordinateSystem();
			this.InteractiveGraphic.Draw();

			return true;
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			return true;
		}

		public override string ToString()
		{
			return "CreatePolygonGraphicState\n";
		}

		private bool AtOrigin(PointF point)
		{
			this.InteractiveGraphic.CoordinateSystem = CoordinateSystem.Destination;
			double distanceToOrigin = Vector.Distance(this.InteractiveGraphic.PolyLine[0], point);
			this.InteractiveGraphic.ResetCoordinateSystem();
			return distanceToOrigin < SNAP_RADIUS;
		}
	}
}