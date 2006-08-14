using System;

namespace ClearCanvas.Desktop
{
	public class ActivationChangedEventArgs : EventArgs
	{
		// Private attributes
		private bool _isActivated;

		// Constructor
		public ActivationChangedEventArgs(bool isActivated)
		{
			_isActivated = isActivated;
		}

		// Properties
		public bool IsActivated { get { return _isActivated; } }
	}
}
