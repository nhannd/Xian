using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer
{
	public class TileEventArgs : CollectionEventArgs<ITile>
	{
		public TileEventArgs()
		{

		}

		public TileEventArgs(ITile tile)
		{
			Platform.CheckForNullReference(tile, "Tile");

			base.Item = tile;
		}

		public ITile Tile { get { return base.Item; } }
	}
}
