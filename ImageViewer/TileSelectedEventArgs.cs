using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	public class TileSelectedEventArgs : EventArgs
	{
		private ITile _selectedTile;

		public TileSelectedEventArgs(
			ITile selectedTile)
		{
			Platform.CheckForNullReference(selectedTile, "selectedTile");
			_selectedTile = selectedTile;
		}

		public ITile SelectedTile
		{
			get { return _selectedTile; }
		}
	}
}
