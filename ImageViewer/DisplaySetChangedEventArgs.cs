using System;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Provides data for the <see cref="Tile.PresentationImageChanged"/> event.
	/// </summary>
	public class DisplaySetChangedEventArgs : EventArgs
	{
		private IDisplaySet _oldDisplaySet;
		private IDisplaySet _newDisplaySet;

		/// <summary>
		/// Initializes a new instance of <see cref="DisplaySetChangedEventArgs"/>.
		/// </summary>
		/// <param name="oldDisplaySet"></param>
		/// <param name="newDisplaySet"></param>
		public DisplaySetChangedEventArgs(
			IDisplaySet oldDisplaySet,
			IDisplaySet newDisplaySet)
		{
			_oldDisplaySet = oldDisplaySet;
			_newDisplaySet = newDisplaySet;
		}

		/// <summary>
		/// Gets the old <see cref="IDisplaySet"/>.
		/// </summary>
		public IDisplaySet OldDisplaySet
		{
			get { return _oldDisplaySet; }
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
