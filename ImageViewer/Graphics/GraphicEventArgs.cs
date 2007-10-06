using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Provides data for the <see cref="GraphicCollection"/> events.
	/// </summary>
	public class GraphicEventArgs : CollectionEventArgs<IGraphic>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="GraphicEventArgs"/>.
		/// </summary>
		public GraphicEventArgs()
		{

		}

		/// <summary>
		/// Initializes a new instance of <see cref="GraphicEventArgs"/> with
		/// a specified <see cref="IGraphic"/>.
		/// </summary>
		/// <param name="graphic"></param>
		public GraphicEventArgs(IGraphic graphic)
		{
			Platform.CheckForNullReference(graphic, "graphic");

			base.Item = graphic;
		}

		/// <summary>
		/// Gets the <see cref="IGraphic"/>
		/// </summary>
		public IGraphic Graphic { get { return base.Item; } }
	}
}
