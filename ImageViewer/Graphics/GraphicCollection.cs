using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
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
		public GraphicCollection()
		{

		}
	}
}
