using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Workstation.Model.Layers
{
	public class GraphicSelectedEventArgs : EventArgs
	{
		private Graphic _selectedGraphic;

		public GraphicSelectedEventArgs(
			Graphic selectedGraphic)
		{
			Platform.CheckForNullReference(selectedGraphic, "selectedGraphic");
			_selectedGraphic = selectedGraphic;
		}

		public Graphic SelectedGraphic
		{
			get { return _selectedGraphic; }
		}
	}
}
