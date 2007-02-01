using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Graphics
{
	public class GraphicCollection : ObservableList<IGraphic, GraphicEventArgs>
	{
		public GraphicCollection()
		{

		}
	}
}
