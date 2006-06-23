using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	public class PresentationImageSelectedEventArgs : EventArgs
	{
		private PresentationImage _selectedPresentationImage;

		public PresentationImageSelectedEventArgs(
			PresentationImage selectedPresentationImage)
		{
			Platform.CheckForNullReference(selectedPresentationImage, "selectedPresentationImage");
			_selectedPresentationImage = selectedPresentationImage;
		}

		public PresentationImage SelectedPresentationImage
		{
			get { return _selectedPresentationImage; }
		}
	}
}
