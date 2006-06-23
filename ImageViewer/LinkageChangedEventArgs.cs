using System;

namespace ClearCanvas.Workstation.Model
{
	public class LinkageChangedEventArgs : EventArgs
	{
		// Private attributes
		private bool m_IsLinked;

		// Constructor
		public LinkageChangedEventArgs(bool isLinked)
		{
			m_IsLinked = isLinked;
		}

		// Properties
		public bool IsLinked { get { return m_IsLinked; } }
	}
}
