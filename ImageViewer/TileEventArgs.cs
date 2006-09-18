using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer
{
	public class TileEventArgs : CollectionEventArgs<Tile>
	{
		public TileEventArgs()
		{

		}

		public TileEventArgs(Tile Tile)
		{
			Platform.CheckForNullReference(Tile, "Tile");

			base.Item = Tile;
		}

		public Tile Tile { get { return base.Item; } }
	}
}
