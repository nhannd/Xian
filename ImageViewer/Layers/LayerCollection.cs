using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Workstation.Model.Layers
{
	/// <summary>
	/// Summary description for LayerCollection.
	/// </summary>
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

		public void Add(CustomLayer customLayer)
		{
			Platform.CheckForNullReference(customLayer, "customLayer");
			base.Add(customLayer);
		}
	}
}
