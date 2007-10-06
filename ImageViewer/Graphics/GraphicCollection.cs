using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A collection of <see cref="IGraphic"/> objects.
	/// </summary>
	public class GraphicCollection : ObservableList<IGraphic, GraphicEventArgs>
	{
		/// <summary>
		/// Instantiates a new instance of <see cref="GraphicCollection"/>.
		/// </summary>
		internal GraphicCollection()
		{

		}
	}
}
