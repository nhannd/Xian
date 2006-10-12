using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	internal class ImageBoxMemento : IMemento
	{
		private IDisplaySet _displaySet;
		private int _rows;
		private int _columns;
		private TileCollection _tiles;
		private MementoList _tileMementos;

		public ImageBoxMemento(
			IDisplaySet displaySet, 
			int rows,
			int columns,
			TileCollection tiles,
			MementoList tileMementos)
		{
			// displaySet can be null, as that would correspond to an
			// empty imageBox
			Platform.CheckNonNegative(rows, "rows");
			Platform.CheckNonNegative(columns, "columns");
			Platform.CheckForNullReference(tiles, "tiles");
			Platform.CheckForNullReference(tileMementos, "tiles");

			_displaySet = displaySet;
			_rows = rows;
			_columns = columns;
			_tiles = tiles;
			_tileMementos = tileMementos;
		}

		public IDisplaySet DisplaySet
		{
			get { return _displaySet; }
		}

		public int Rows
		{
			get { return _rows; }
		}

		public int Columns
		{
			get { return _columns; }
		}

		public TileCollection Tiles
		{
			get { return _tiles; }
		}

		public MementoList TileMementos
		{
			get { return _tileMementos; }
		}
	}
}
