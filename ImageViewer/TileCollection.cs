using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Summary description for TileCollection.
	/// </summary>
	public class TileCollection : ObservableList<Tile, TileEventArgs>
	{
		public TileCollection()
		{

		}

		/// <summary>
		/// Creates a copy of the object.
		/// </summary>
		/// <param name="collection"></param>
		/// <remarks>
		/// Creates a <i>shallow</i> copy.  That is, only references to objects
		/// in the collection are copied.
		/// </remarks>
		public TileCollection(ObservableList<Tile, TileEventArgs> collection) 
			: base(collection)
		{
		}
	}
}
