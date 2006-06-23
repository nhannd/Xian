using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Workstation.Model
{
	public class ImageBoxSelectedEventArgs : EventArgs
	{
		private ImageBox _selectedImageBox;

		public ImageBoxSelectedEventArgs(
			ImageBox selectedImageBox)
		{
			Platform.CheckForNullReference(selectedImageBox, "selectedImageBox");
			_selectedImageBox = selectedImageBox;
		}

		public ImageBox SelectedImageBox
		{
			get { return _selectedImageBox; }
		}
	}
}
