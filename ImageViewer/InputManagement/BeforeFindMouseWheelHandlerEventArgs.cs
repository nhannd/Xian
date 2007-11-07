using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.InputManagement
{
	/// <summary>
	/// Used by the framework to notify subscribers that it is about to attempt to find
	/// a valid <see cref="IMouseWheelHandler"/> for either <see cref="IMouseWheelHandler.Start"/> perations.
	/// </summary>
	public class BeforeFindMouseWheelHandlerEventArgs : EventArgs
	{
		private ITile _tile;
		private bool _cancelled;

		internal BeforeFindMouseWheelHandlerEventArgs(ITile tile)
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
		/// Cancels the framework's attempt to find an <see cref="IMouseWheelHandler"/>.
		/// </summary>
		public void Cancel()
		{
			_cancelled = true;
		}
	}
}
