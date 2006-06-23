using System;

namespace ClearCanvas.Desktop
{
	public class ActivationChangedEventArgs : EventArgs
	{
		// Private attributes
		private bool m_IsActivated;

		// Constructor
		public ActivationChangedEventArgs(bool isActivated)
		{
			m_IsActivated = isActivated;
		}

		// Properties
		public bool IsActivated { get { return m_IsActivated; } }
	}
}
