using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Layers
{
	public class LayerEventArgs : CollectionEventArgs<Layer>
	{
		public LayerEventArgs()
		{

		}

		public LayerEventArgs(Layer layer)
		{
			Platform.CheckForNullReference(layer, "Layer");

			base.Item = layer;
		}

		public Layer Layer { get { return base.Item; } }
	}
}
