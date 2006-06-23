using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Workstation.Model.Layers
{
	public class GraphicLayerSelectedEventArgs : EventArgs
	{
		private GraphicLayer _selectedGraphicLayer;

		public GraphicLayerSelectedEventArgs(
			GraphicLayer selectedGraphicLayer)
		{
			Platform.CheckForNullReference(selectedGraphicLayer, "selectedGraphicLayer");
			_selectedGraphicLayer = selectedGraphicLayer;
		}

		public GraphicLayer SelectedGraphicLayer
		{
			get { return _selectedGraphicLayer; }
		}
	}
}
