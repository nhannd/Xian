using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Workstation.Model.Layers
{
	public class ImageLayerSelectedEventArgs : EventArgs
	{
		private ImageLayer _selectedImageLayer;

		public ImageLayerSelectedEventArgs(
			ImageLayer selectedImageLayer)
		{
			Platform.CheckForNullReference(selectedImageLayer, "selectedImageLayer");
			_selectedImageLayer = selectedImageLayer;
		}

		public ImageLayer SelectedImageLayer
		{
			get { return _selectedImageLayer; }
		}
	}
}
