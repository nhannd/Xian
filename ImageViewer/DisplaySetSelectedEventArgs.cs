using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Provides data for the <see cref="DisplaySetSelected"/> event.
	/// </summary>
	public class DisplaySetSelectedEventArgs : EventArgs
	{
		private IDisplaySet _selectedDisplaySet;

		/// <summary>
		/// Initializes a new instance of <see cref="DisplaySetSelectedEventArgs"/> with
		/// a specified <see cref="IDisplaySet"/>
		/// </summary>
		/// <param name="selectedDisplaySet"></param>
		public DisplaySetSelectedEventArgs(
			IDisplaySet selectedDisplaySet)
		{
			Platform.CheckForNullReference(selectedDisplaySet, "selectedDisplaySet");
			_selectedDisplaySet = selectedDisplaySet;
		}

		/// <summary>
		/// Gets the selected display set.
		/// </summary>
		public IDisplaySet SelectedDisplaySet
		{
			get { return _selectedDisplaySet; }
		}
	}
}
