using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Provides data for the <see cref="TileCollection"/> events.
	/// </summary>
	public class TileEventArgs : CollectionEventArgs<ITile>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="TileEventArgs"/>.
		/// </summary>
		public TileEventArgs()
		{

		}

		/// <summary>
		/// Initializes a new instance of <see cref="TileEventArgs"/> with
		/// a specified <see cref="ITile"/>
		/// </summary>
		/// <param name="tile"></param>
		public TileEventArgs(ITile tile)
		{
			Platform.CheckForNullReference(tile, "Tile");

			base.Item = tile;
		}

		/// <summary>
		/// Gets the <see cref="ITile"/>.
		/// </summary>
		public ITile Tile { get { return base.Item; } }
	}
}
