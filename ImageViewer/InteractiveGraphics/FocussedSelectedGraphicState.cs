#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
		/// is clicked on.
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
