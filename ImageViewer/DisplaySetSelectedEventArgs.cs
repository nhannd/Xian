using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Workstation.Model
{
	public class DisplaySetSelectedEventArgs : EventArgs
	{
		private DisplaySet _selectedDisplaySet;

		public DisplaySetSelectedEventArgs(
			DisplaySet selectedDisplaySet)
		{
			Platform.CheckForNullReference(selectedDisplaySet, "selectedDisplaySet");
			_selectedDisplaySet = selectedDisplaySet;
		}

		public DisplaySet SelectedDisplaySet
		{
			get { return _selectedDisplaySet; }
		}
	}
}
