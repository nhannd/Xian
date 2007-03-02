using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Provides data for the <see cref="EventBroker.TileSelected"/> event.
	/// </summary>
	public class TileSelectedEventArgs : EventArgs
	{
		private ITile _selectedTile;

		internal TileSelectedEventArgs(
			ITile selectedTile)
		{
			Platform.CheckForNullReference(selectedTile, "selectedTile");
			_selectedTile = selectedTile;
		}

		/// <summary>
		/// Gets the selected <see cref="ITile"/>.
		/// </summary>
		public ITile SelectedTile
		{
			get { return _selectedTile; }
		}
	}
}
