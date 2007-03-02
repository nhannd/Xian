using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A collection of <see cref="ITile"/> objects.
	/// </summary>
	public class TileCollection : ObservableList<ITile, TileEventArgs>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="TileCollection"/>.
		/// </summary>
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
		public TileCollection(ObservableList<ITile, TileEventArgs> collection) 
			: base(collection)
		{
		}
	}
}
