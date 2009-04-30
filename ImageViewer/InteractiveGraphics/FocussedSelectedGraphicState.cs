#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
	/// <summary>
	/// Represents the 'focussed-selected' graphic state.
	/// </summary>
	/// <remarks>
	/// This state is different from <see cref="SelectedGraphicState"/> in 
	/// that it is entered when the
	/// <see cref="IStandardStatefulGraphic"/> is selected (i.e. clicked on)
	/// <i>and</i> focussed (i.e. hovered over), whereas <see cref="SelectedGraphicState"/>
	/// is entered only when a graphic is selected, but not focussed.
	/// </remarks>
	public class FocussedSelectedGraphicState : StandardGraphicState
	{
		/// <summary>
		/// Initializes a new instance of <see cref="FocussedSelectedGraphicState"/>.
		/// </summary>
		/// <param name="standardStatefulGraphic"></param>
		public FocussedSelectedGraphicState(IStandardStatefulGraphic standardStatefulGraphic)
			: base(standardStatefulGraphic)
		{
		}

		/// <summary>
		/// Called by the framework when the associated <see cref="IStandardStatefulGraphic"/>
		/// is clicked on and results in a transition to the <see cref="MoveGraphicState"/>.
		/// </summary>
		/// <param name="mouseInformation"></param>
		/// <returns></returns>
		public override bool Start(IMouseInformation mouseInformation)
		{
			//We should never actually get to here, but if we did, this should happen.
			this.StatefulGraphic.State = this.StatefulGraphic.CreateSelectedState();
			return false;
		}

		/// <summary>
		/// Called by the framework when the mouse is moving and results in a transition 
		/// to the <see cref="SelectedGraphicState"/> when
		/// the mouse is no longer hovering over the associated 
		/// <see cref="IStandardStatefulGraphic"/>.
		/// </summary>
		public override bool Track(IMouseInformation mouseInformation)
		{
			if (!this.StatefulGraphic.HitTest(mouseInformation.Location))
			{
				this.StatefulGraphic.State = this.StatefulGraphic.CreateSelectedState();
				return false;
			}

			return true;
		}
	}
}
