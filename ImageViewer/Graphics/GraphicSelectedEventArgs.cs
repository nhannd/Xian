using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Provides data for the <see cref="EventBroker.GraphicSelected"/> event.
	/// </summary>
	public class GraphicSelectedEventArgs : EventArgs
	{
		private ISelectableGraphic _selectedGraphic;

		internal GraphicSelectedEventArgs(
			ISelectableGraphic selectedGraphic)
		{
			//Platform.CheckForNullReference(selectedGraphic, "selectedGraphic");
			_selectedGraphic = selectedGraphic;
		}

		/// <summary>
		/// Gets the selected <see cref="IGraphic"/>.
		/// </summary>
		public ISelectableGraphic SelectedGraphic
		{
			get { return _selectedGraphic; }
		}
	}
}
