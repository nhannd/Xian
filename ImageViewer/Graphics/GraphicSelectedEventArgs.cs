using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Graphics
{
	public class GraphicSelectedEventArgs : EventArgs
	{
		private ISelectableGraphic _selectedGraphic;

		public GraphicSelectedEventArgs(
			ISelectableGraphic selectedGraphic)
		{
			//Platform.CheckForNullReference(selectedGraphic, "selectedGraphic");
			_selectedGraphic = selectedGraphic;
		}

		public ISelectableGraphic SelectedGraphic
		{
			get { return _selectedGraphic; }
		}
	}
}
