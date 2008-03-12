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
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// Represents the 'move control point' graphic state.
	/// </summary>
	/// <remarks>
	/// This state is entered when the user clicks on <see cref="ControlPoint"/>
	/// on an <see cref="InteractiveGraphic"/>.
	/// </remarks>
	public class MoveControlPointGraphicState : StandardGraphicState
	{
		private PointF _currentPoint;
		private PointF _startPoint;
		private int _controlPointIndex;

		/// <summary>
		/// Initializes a new instance of <see cref="MoveControlPointGraphicState"/>.
		/// </summary>
		/// <param name="interactiveGraphic"></param>
		/// <param name="controlPointIndex"></param>
		public MoveControlPointGraphicState(
			InteractiveGraphic interactiveGraphic, 
			int controlPointIndex) : base(interactiveGraphic)
		{
			Platform.CheckForNullReference(interactiveGraphic, "interactiveGraphic");
			Platform.CheckNonNegative(controlPointIndex, "controlPointIndex");

			_controlPointIndex = controlPointIndex;
		}

		/// <summary>
		/// Gets the associated <see cref="InteractiveGraphic"/>.
		/// </summary>
		protected InteractiveGraphic InteractiveGraphic
		{
			get { return this.StandardStatefulGraphic as InteractiveGraphic; }
		}

		/// <summary>
		/// Called by the framework when a <see cref="ControlPoint"/> is clicked.
		/// </summary>
		/// <param name="mouseInformation"></param>
		/// <returns></returns>
		public override bool Start(IMouseInformation mouseInformation)
		{
			base.LastPoint = this.InteractiveGraphic.SpatialTransform.ConvertToSource(mouseInformation.Location);
			_startPoint = _currentPoint = base.LastPoint;

			this.InteractiveGraphic.Draw();

			return true;
		}

		/// <summary>
		/// Called by the framework when the mouse is moving while
		/// a mouse button is pressed and results in the moving
		/// of the selected <see cref="ControlPoint"/>.
		/// </summary>
		/// <param name="mouseInformation"></param>
		/// <returns></returns>
		public override bool Track(IMouseInformation mouseInformation)
		{
			_currentPoint = this.InteractiveGraphic.SpatialTransform.ConvertToSource(mouseInformation.Location);
			this.InteractiveGraphic.CoordinateSystem = CoordinateSystem.Source;
			this.InteractiveGraphic.ControlPoints[_controlPointIndex] = _currentPoint;
			this.InteractiveGraphic.ResetCoordinateSystem();
			this.InteractiveGraphic.Draw();

			base.LastPoint = _currentPoint;

			return true;
		}

		/// <summary>
		/// Called by the framework when the mouse button is released
		/// and results in a transition back to the <see cref="FocussedSelectedGraphicState"/>.
		/// </summary>
		/// <param name="mouseInformation"></param>
		/// <returns></returns>
		public override bool Stop(IMouseInformation mouseInformation)
		{
			Cancel();

			this.InteractiveGraphic.Draw();
			return false;
		}

		/// <summary>
		/// Cancels the move operation and results in a transition back
		/// to the <see cref="FocussedSelectedGraphicState"/>.
		/// </summary>
		public override void Cancel()
		{
			this.StandardStatefulGraphic.State = this.StandardStatefulGraphic.CreateFocussedSelectedState();
		}

		/// <summary>
		/// Returns a string describing this graphic state.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return "Move Control Point State\n";
		}
	}
}
