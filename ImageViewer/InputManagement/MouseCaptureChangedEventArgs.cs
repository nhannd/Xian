using System;

namespace ClearCanvas.ImageViewer.InputManagement
{
	/// <summary>
	/// Event fired when mouse capture is gained or lost.
	/// </summary>
	public class MouseCaptureChangedEventArgs : EventArgs
	{
		private readonly bool _gained;
		private readonly ITile _tile;

		internal MouseCaptureChangedEventArgs(ITile tile, bool gained)
		{
			_gained = gained;
			_tile = tile;
		}

		/// <summary>
		/// Gets the affected tile.
		/// </summary>
		public ITile Tile
		{
			get { return _tile; }
		}
	
		/// <summary>
		/// True if mouse capture has been gained, false if it was lost.
		/// </summary>
		public bool Gained
		{
			get { return _gained; }
		}
	}
}
