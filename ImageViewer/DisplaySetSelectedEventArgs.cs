using System;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Provides data for the <see cref="EventBroker.DisplaySetSelected"/> event.
	/// </summary>
	public class DisplaySetSelectedEventArgs : EventArgs
	{
		private IDisplaySet _selectedDisplaySet;

		internal DisplaySetSelectedEventArgs(
			IDisplaySet selectedDisplaySet)
		{
			Platform.CheckForNullReference(selectedDisplaySet, "selectedDisplaySet");
			_selectedDisplaySet = selectedDisplaySet;
		}

		/// <summary>
		/// Gets the selected <see cref="IDisplaySet"/>.
		/// </summary>
		public IDisplaySet SelectedDisplaySet
		{
			get { return _selectedDisplaySet; }
		}
	}
}
