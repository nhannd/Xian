using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Application;

namespace ClearCanvas.Workstation.Model
{
	/// <summary>
	/// Summary description for ImageBoxMemento.
	/// </summary>
	internal class ImageBoxMemento : IMemento
	{
		private DisplaySet m_DisplaySet;
		private ClientArea m_ClientArea;
		private int m_Rows;
		private int m_Columns;
		private TileCollection m_Tiles;
		private MementoList m_TileMementos;

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

			m_DisplaySet = displaySet;
			m_ClientArea = clientArea;
			m_Rows = rows;
			m_Columns = columns;
			m_Tiles = tiles;
			m_TileMementos = tileMementos;
		}

		public DisplaySet DisplaySet
		{
			get { return m_DisplaySet; }
		}

		public ClientArea ClientArea
		{
			get	{ return m_ClientArea; }
		}

		public int Rows
		{
			get { return m_Rows; }
		}

		public int Columns
		{
			get { return m_Columns; }
		}

		public TileCollection Tiles
		{
			get { return m_Tiles; }
		}

		public MementoList TileMementos
		{
			get { return m_TileMementos; }
		}
	}
}
