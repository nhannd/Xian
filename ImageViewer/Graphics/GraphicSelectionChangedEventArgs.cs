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
	/// Provides data for the <see cref="EventBroker.GraphicSelectionChanged"/> event.
	/// </summary>
	public class GraphicSelectionChangedEventArgs : EventArgs
	{
		private readonly ISelectableGraphic _selectedGraphic;
		private readonly ISelectableGraphic _deselectedGraphic;

		/// <summary>
		/// Intializes a new instance of <see cref="GraphicSelectionChangedEventArgs"/>.
		/// </summary>
		/// <param name="selectedGraphic">The graphic that was selected. Can be <b>null</b> if there is no currently selected graphic.</param>
		/// <param name="deselectedGraphic">The graphic that was previously selected. Can be <b>null</b> if there was previously no selected graphic.</param>
		internal GraphicSelectionChangedEventArgs(ISelectableGraphic selectedGraphic, ISelectableGraphic deselectedGraphic)
		{
			_selectedGraphic = selectedGraphic;
			_deselectedGraphic = deselectedGraphic;
		}

		/// <summary>
		/// Gets the selected <see cref="IGraphic"/>. Can be <b>null</b> if there is no currently selected graphic.
		/// </summary>
		public ISelectableGraphic SelectedGraphic
		{
			get { return _selectedGraphic; }
		}

		/// <summary>
		/// Gets the deselected <see cref="IGraphic"/>. Can be <b>null</b> if there was previously no selected graphic.
		/// </summary>
		public ISelectableGraphic DeselectedGraphic
		{
			get { return _deselectedGraphic; }
		}
	}
}