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
		internal TileCollection()
		{

		}
	}
}
