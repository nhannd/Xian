using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Provides data for the <see cref="EventBroker.ImageBoxSelected"/> event.
	/// </summary>
	public class ImageBoxSelectedEventArgs : EventArgs
	{
		private IImageBox _selectedImageBox;

		internal ImageBoxSelectedEventArgs(
			IImageBox selectedImageBox)
		{
			Platform.CheckForNullReference(selectedImageBox, "selectedImageBox");
			_selectedImageBox = selectedImageBox;
		}

		/// <summary>
		/// Gets the selected <see cref="IImageBox"/>.
		/// </summary>
		public IImageBox SelectedImageBox
		{
			get { return _selectedImageBox; }
		}
	}
}
