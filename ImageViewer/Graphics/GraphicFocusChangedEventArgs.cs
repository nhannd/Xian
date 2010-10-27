#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Provides data for the <see cref="EventBroker.GraphicFocusChanged"/> event.
	/// </summary>
	public class GraphicFocusChangedEventArgs : EventArgs
	{
		private readonly IFocussableGraphic _focusedGraphic;
		private readonly IFocussableGraphic _unfocusedGraphic;

		/// <summary>
		/// Intializes a new instance of <see cref="GraphicFocusChangedEventArgs"/>.
		/// </summary>
		/// <param name="focusedGraphic">The graphic that was focused. Can be <b>null</b> if there is no currently focused graphic.</param>
		/// <param name="unfocusedGraphic">The graphic that was previously focused. Can be <b>null</b> if there was previously no focused graphic.</param>
		internal GraphicFocusChangedEventArgs(IFocussableGraphic focusedGraphic, IFocussableGraphic unfocusedGraphic)
		{
			_focusedGraphic = focusedGraphic;
			_unfocusedGraphic = unfocusedGraphic;
		}

		/// <summary>
		/// Gets the focused <see cref="IGraphic"/>. Can be <b>null</b> if there is no currently focused graphic.
		/// </summary>
		public IFocussableGraphic FocusedGraphic
		{
			get { return _focusedGraphic; }
		}

		/// <summary>
		/// Gets the deselected <see cref="IGraphic"/>. Can be <b>null</b> if there was previously no focused graphic.
		/// </summary>
		public IFocussableGraphic UnfocusedGraphic
		{
			get { return _unfocusedGraphic; }
		}
	}
}