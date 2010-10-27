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
	/// Represents the 'focussed' graphic state.
	/// </summary>
	/// <remarks>
	/// This state is entered when the mouse hovers over a 
	/// <see cref="IStandardStatefulGraphic"/>.
	/// </remarks>
	public class FocussedGraphicState : StandardGraphicState
	{
		/// <summary>
		/// Initializes a new instance of <see cref="FocussedGraphicState"/>.
		/// </summary>
		/// <param name="standardStatefulGraphic"></param>
		public FocussedGraphicState(IStandardStatefulGraphic standardStatefulGraphic)
			: base(standardStatefulGraphic)
		{
		}

		/// <summary>
		/// Called by the framework when the associated <see cref="IStandardStatefulGraphic"/>
		/// is clicked on and results in a transition to the <see cref="FocussedSelectedGraphicState"/>.
		/// </summary>
		/// <param name="mouseInformation"></param>
		/// <returns></returns>
		public override bool Start(IMouseInformation mouseInformation)
		{
			if (this.StatefulGraphic.HitTest(mouseInformation.Location))
			{
				this.StatefulGraphic.State = this.StatefulGraphic.CreateFocussedSelectedState();
				this.StatefulGraphic.State.Start(mouseInformation);

				return true;
			}

			//We should never actually get to here, but if we did, this should happen.
			base.StatefulGraphic.State = this.StatefulGraphic.CreateInactiveState();
			return false;
		}

		/// <summary>
		/// Called by the framework when the mouse is moving and results in a transition 
		/// to the <see cref="InactiveGraphicState"/> when
		/// the mouse is no longer hovering over the associated 
		/// <see cref="IStandardStatefulGraphic"/>.
		/// </summary>
		/// <param name="mouseInformation"></param>
		/// <returns></returns>
		public override bool Track(IMouseInformation mouseInformation)
		{
			if (!this.StatefulGraphic.HitTest(mouseInformation.Location))
			{
				this.StatefulGraphic.State = this.StatefulGraphic.CreateInactiveState();
				return false;
			}

			return true;
		}
	}
}
