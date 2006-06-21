using System;

namespace ClearCanvas.Common.Application
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
