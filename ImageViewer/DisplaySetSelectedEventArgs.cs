using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	public class DisplaySetSelectedEventArgs : EventArgs
	{
		private IDisplaySet _selectedDisplaySet;

		public DisplaySetSelectedEventArgs(
			IDisplaySet selectedDisplaySet)
		{
			Platform.CheckForNullReference(selectedDisplaySet, "selectedDisplaySet");
			_selectedDisplaySet = selectedDisplaySet;
		}

		public IDisplaySet SelectedDisplaySet
		{
			get { return _selectedDisplaySet; }
		}
	}
}
