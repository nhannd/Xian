using System;

namespace ClearCanvas.Desktop
{
	public class SelectionChangedEventArgs : EventArgs
	{
		// Private attributes
		private bool _selected;

		// Constructor
		public SelectionChangedEventArgs(bool selected)
		{
			_selected = selected;
		}

		// Properties
		public bool Selected { get { return _selected; } }
	}
}
