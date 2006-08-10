using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Summary description for ImageBoxMemento.
	/// </summary>
	internal class ImageBoxMemento : IMemento
	{
		private DisplaySet _DisplaySet;
		private ClientArea _ClientArea;
		private int _Rows;
		private int _Columns;
		private TileCollection _Tiles;
		private MementoList _TileMementos;

		public ImageBoxMemento(
			DisplaySet displaySet, 
			ClientArea clientArea,
			int rows,
			int columns,
			TileCollection tiles,
			MementoList tileMementos)
		{
			// displaySet can be null, as that would correspond to an
			// empty imageBox
			Platform.CheckForNullReference(clientArea, "clientArea");
			Platform.CheckNonNegative(rows, "rows");
			Platform.CheckNonNegative(columns, "columns");
			Platform.CheckForNullReference(tiles, "tiles");
			Platform.CheckForNullReference(tileMementos, "tiles");

			_DisplaySet = displaySet;
			_ClientArea = clientArea;
			_Rows = rows;
			_Columns = columns;
			_Tiles = tiles;
			_TileMementos = tileMementos;
		}

		public DisplaySet DisplaySet
		{
			get { return _DisplaySet; }
		}

		public ClientArea ClientArea
		{
			get	{ return _ClientArea; }
		}

		public int Rows
		{
			get { return _Rows; }
		}

		public int Columns
		{
			get { return _Columns; }
		}

		public TileCollection Tiles
		{
			get { return _Tiles; }
		}

		public MementoList TileMementos
		{
			get { return _TileMementos; }
		}
	}
}
