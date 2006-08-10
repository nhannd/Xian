using System;

namespace ClearCanvas.Desktop
{
	public class ActivationChangedEventArgs : EventArgs
	{
		// Private attributes
		private bool _IsActivated;

		// Constructor
		public ActivationChangedEventArgs(bool isActivated)
		{
			_IsActivated = isActivated;
		}

		// Properties
		public bool IsActivated { get { return _IsActivated; } }
	}
}
