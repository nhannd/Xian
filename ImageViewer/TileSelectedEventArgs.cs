using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Workstation.Model
{
	public class TileSelectedEventArgs : EventArgs
	{
		private Tile _selectedTile;

		public TileSelectedEventArgs(
			Tile selectedTile)
		{
			Platform.CheckForNullReference(selectedTile, "selectedTile");
			_selectedTile = selectedTile;
		}

		public Tile SelectedTile
		{
			get { return _selectedTile; }
		}
	}
}
