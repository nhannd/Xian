using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Provides data for the <see cref="EventBroker.GraphicSelectionChanged"/> event.
	/// </summary>
	public class GraphicSelectionChangedEventArgs : EventArgs
	{
		private ISelectableGraphic _selectedGraphic;
		private ISelectableGraphic _deselectedGraphic;

		/// <summary>
		/// Intializes a new instance of <see cref="GraphicSelectionChangedEventArgs"/>.
		/// </summary>
		/// <param name="selectedGraphic">The graphic that was selected. Can
		/// be <b>null</b> if 
		/// </param>
		/// <param name="deselectedGraphic"></param>
		internal GraphicSelectionChangedEventArgs(
			ISelectableGraphic selectedGraphic,
			ISelectableGraphic deselectedGraphic)
		{


			_selectedGraphic = selectedGraphic;
			_deselectedGraphic = deselectedGraphic;
		}

		/// <summary>
		/// Gets the selected <see cref="IGraphic"/>.
		/// </summary>
		public ISelectableGraphic SelectedGraphic
		{
			get { return _selectedGraphic; }
		}

		/// <summary>
		/// Gets the deselected <see cref="IGraphic"/>.
		/// </summary>
		public ISelectableGraphic DeselectedGraphic
		{
			get { return _deselectedGraphic; }
		}
	}
}
