using System;
using ClearCanvas.ImageViewer;

namespace ClearCanvas.ImageViewer.InputManagement
{
	/// <summary>
	/// Used by the framework to notify subscribers that it is about to attempt to find
	/// a valid <see cref="IMouseButtonHandler"/> for either <see cref="IMouseButtonHandler.Start"/> 
	/// or <see cref="IMouseButtonHandler.Track"/> operations.
	/// </summary>
	public class BeforeFindMouseButtonHandlerEventArgs : EventArgs
	{
		private ITile _tile;
		private bool _cancelled;

		internal BeforeFindMouseButtonHandlerEventArgs(ITile tile)
		{
			_tile = tile;
			_cancelled = false;
		}

		internal bool Cancelled
		{
			get { return _cancelled; }
		}

		/// <summary>
		/// Gets the <see cref="ITile"/> the mouse is currently in.
		/// </summary>
		public ITile Tile
		{
			get { return _tile; }
		}

		/// <summary>
		/// Cancels the framework's attempt to find an <see cref="IMouseButtonHandler"/>.
		/// </summary>
		public void Cancel()
		{
			_cancelled = true;
		}
	}
}
