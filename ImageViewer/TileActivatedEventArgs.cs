using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	public class TileActivatedEventArgs : EventArgs
	{
		private ITile _activatedTile;

		public TileActivatedEventArgs(ITile activatedTile)
		{
			Platform.CheckForNullReference(activatedTile, "activatedTile");
			_activatedTile = activatedTile;
		}

		public ITile ActivatedTile
		{
			get { return _activatedTile; }
		}
	}
}
