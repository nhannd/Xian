using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	public class ImageBoxSelectedEventArgs : EventArgs
	{
		private IImageBox _selectedImageBox;

		public ImageBoxSelectedEventArgs(
			IImageBox selectedImageBox)
		{
			Platform.CheckForNullReference(selectedImageBox, "selectedImageBox");
			_selectedImageBox = selectedImageBox;
		}

		public IImageBox SelectedImageBox
		{
			get { return _selectedImageBox; }
		}
	}
}
