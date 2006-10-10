using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Layers
{
	public class LayerCollection : BaseLayerCollection
	{
		public LayerCollection()
		{

		}

		public void Add(LayerGroup layerGroup)
		{
			Platform.CheckForNullReference(layerGroup, "layerGroup");
			base.Add(layerGroup);
		}

		public void Add(ImageLayer imageLayer)
		{
			Platform.CheckForNullReference(imageLayer, "imageLayer");
			base.Add(imageLayer);
		}

		public void Add(GraphicLayer graphicLayer)
		{
			Platform.CheckForNullReference(graphicLayer, "graphicLayer");
			base.Add(graphicLayer);
		}

	}
}
