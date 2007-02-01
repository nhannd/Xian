using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Graphics
{
	public class GraphicEventArgs : CollectionEventArgs<IGraphic>
	{
		public GraphicEventArgs()
		{

		}

		public GraphicEventArgs(IGraphic graphic)
		{
			Platform.CheckForNullReference(graphic, "graphic");

			base.Item = graphic;
		}

		public IGraphic Graphic { get { return base.Item; } }
	}
}
