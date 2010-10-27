#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// Defines a stateful graphic with a number of standard states.
	/// </summary>
	public interface IStandardStatefulGraphic : IStatefulGraphic
	{
		/// <summary>
		/// Creates an inactive <see cref="GraphicState"/> for the current graphic.
		/// </summary>
		/// <returns>An inactive <see cref="GraphicState"/> for the current graphic.</returns>
		GraphicState CreateInactiveState();

		/// <summary>
		/// Creates a focussed <see cref="GraphicState"/> for the current graphic.
		/// </summary>
		/// <returns>An inactive <see cref="GraphicState"/> for the current graphic.</returns>
		GraphicState CreateFocussedState();

		/// <summary>
		/// Creates a selected <see cref="GraphicState"/> for the current graphic.
		/// </summary>
		/// <returns>An inactive <see cref="GraphicState"/> for the current graphic.</returns>
		GraphicState CreateSelectedState();

		/// <summary>
		/// Creates a focussed and selected <see cref="GraphicState"/> for the current graphic.
		/// </summary>
		/// <returns>An inactive <see cref="GraphicState"/> for the current graphic.</returns>
		GraphicState CreateFocussedSelectedState();
	}
}
