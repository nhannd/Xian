using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Workstation.Model.Layers
{
	public class LayerGroupSelectedEventArgs : EventArgs
	{
		private LayerGroup _selectedLayerGroup;

		public LayerGroupSelectedEventArgs(
			LayerGroup selectedLayerGroup)
		{
			Platform.CheckForNullReference(selectedLayerGroup, "selectedLayerGroup");
			_selectedLayerGroup = selectedLayerGroup;
		}

		public LayerGroup SelectedLayerGroup
		{
			get { return _selectedLayerGroup; }
		}
	}
}
