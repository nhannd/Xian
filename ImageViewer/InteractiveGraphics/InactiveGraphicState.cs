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
	/// Represents the 'inactive' graphic state.
	/// </summary>
	/// <remarks>
	/// This state is entered when the mouse has moved away from a
	/// <see cref="IStandardStatefulGraphic"/> that is not currently
	/// selected.
	/// </remarks>
	public class InactiveGraphicState : StandardGraphicState
	{
		/// <summary>
		/// Initializes a new instance of <see cref="InactiveGraphicState"/>.
		/// </summary>
		/// <param name="standardStatefulGraphic"></param>
		public InactiveGraphicState(IStandardStatefulGraphic standardStatefulGraphic)
			: base(standardStatefulGraphic)
		{
		}

		/// <summary>
		/// Called by the framework when the mouse is moving and results in a transition 
		/// to the <see cref="FocussedGraphicState"/> when
		/// the mouse hovers over the associated <see cref="IStandardStatefulGraphic"/>.
		/// </summary>
		/// <param name="mouseInformation"></param>
		/// <returns></returns>
		public override bool Track(IMouseInformation mouseInformation)
		{
			// If mouse is over object, transition to focused state
			if (this.StatefulGraphic.HitTest(mouseInformation.Location))
			{
				this.StatefulGraphic.State = this.StatefulGraphic.CreateFocussedState();
				return true;
			}

			return false;
		}
	}
}
