using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Workstation.Model.Layers
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
