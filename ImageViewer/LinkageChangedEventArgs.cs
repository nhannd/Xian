using System;

namespace ClearCanvas.ImageViewer
{
	public class LinkageChangedEventArgs : EventArgs
	{
		// Private attributes
		private bool _isLinked;

		// Constructor
		public LinkageChangedEventArgs(bool isLinked)
		{
			_isLinked = isLinked;
		}

		// Properties
		public bool IsLinked { get { return _isLinked; } }
	}
}
