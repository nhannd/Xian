using System;

namespace ClearCanvas.ImageViewer
{
	public class LinkageChangedEventArgs : EventArgs
	{
		// Private attributes
		private bool _IsLinked;

		// Constructor
		public LinkageChangedEventArgs(bool isLinked)
		{
			_IsLinked = isLinked;
		}

		// Properties
		public bool IsLinked { get { return _IsLinked; } }
	}
}
