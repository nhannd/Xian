using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Provides data for the <see cref="EventBroker.DisplaySetChanging"/> event.
	/// </summary>
	public class DisplaySetChangingEventArgs : EventArgs
	{
		private IDisplaySet _currentDisplaySet;
		private IDisplaySet _newDisplaySet;

		/// <summary>
		/// Initializes a new instance of <see cref="DisplaySetChangingEventArgs"/>.
		/// </summary>
		public DisplaySetChangingEventArgs(
			IDisplaySet currentDisplaySet,
			IDisplaySet newDisplaySet)
		{
			_currentDisplaySet = currentDisplaySet;
			_newDisplaySet = newDisplaySet;
		}

		/// <summary>
		/// Gets the current <see cref="IDisplaySet"/>.
		/// </summary>
		public IDisplaySet CurrentDisplaySet
		{
			get { return _currentDisplaySet; }
		}

		/// <summary>
		/// Gets the new <see cref="IDisplaySet"/>.
		/// </summary>
		public IDisplaySet NewDisplaySet
		{
			get { return _newDisplaySet; }
		}
	}
}
