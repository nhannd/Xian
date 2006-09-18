using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Layers
{
	/// <summary>
	/// Summary description for BaseLayerCollection.
	/// </summary>
	public class BaseLayerCollection : ObservableList<Layer, LayerEventArgs>
	{
		public BaseLayerCollection()
		{

		}
	}
}
