#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
	/// Represents the 'selected' graphic state.
	/// </summary>
	/// <remarks>
	/// This state is different from <see cref="FocussedSelectedGraphicState"/> in 
	/// that it is entered when the
	/// <see cref="IStandardStatefulGraphic"/> is selected (i.e. clicked on)
	/// but <i>not</i> focussed (i.e. hovered over), whereas <see cref="FocussedSelectedGraphicState"/>
	/// is entered only when a graphic is selected <i>and</i> focussed.
	/// </remarks>
	public class SelectedGraphicState : StandardGraphicState
	{
		/// <summary>
		/// Initializes a new instance of <see cref="SelectedGraphicState"/>.
		/// </summary>
		/// <param name="standardStatefulGraphic"></param>
		public SelectedGraphicState(IStandardStatefulGraphic standardStatefulGraphic)
			: base(standardStatefulGraphic)
		{
		}

		/// <summary>
		/// Called by the framework when the user clicks away from the 
		/// associated <see cref="IStandardStatefulGraphic"/>
		/// and results in a transition to the <see cref="InactiveGraphicState"/>.
		/// </summary>
		/// <param name="mouseInformation"></param>
		/// <returns></returns>
		public override bool Start(IMouseInformation mouseInformation)
		{
			if (!this.StatefulGraphic.HitTest(mouseInformation.Location))
			{
				this.StatefulGraphic.State = this.StatefulGraphic.CreateInactiveState();
				return false;
			}

			return true;
		}

		/// <summary>
		/// Called by the framework when the mouse is moving and results in a transition 
		/// to the <see cref="FocussedGraphicState"/> when
		/// the mouse hovers over the associated 
		/// <see cref="IStandardStatefulGraphic"/>.
		/// </summary>
		/// <param name="mouseInformation"></param>
		/// <returns></returns>
		public override bool Track(IMouseInformation mouseInformation)
		{
			if (this.StatefulGraphic.HitTest(mouseInformation.Location))
			{
				this.StatefulGraphic.State = this.StatefulGraphic.CreateFocussedSelectedState();
				return true;
			}

			return false;
		}
	}
}
