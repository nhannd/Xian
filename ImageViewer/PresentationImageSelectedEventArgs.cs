using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	public class PresentationImageSelectedEventArgs : EventArgs
	{
		private IPresentationImage _selectedPresentationImage;

		public PresentationImageSelectedEventArgs(
			IPresentationImage selectedPresentationImage)
		{
			Platform.CheckForNullReference(selectedPresentationImage, "selectedPresentationImage");
			_selectedPresentationImage = selectedPresentationImage;
		}

		public IPresentationImage SelectedPresentationImage
		{
			get { return _selectedPresentationImage; }
		}
	}
}
