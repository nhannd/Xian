using System;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Provides data for the <see cref="EventBroker.PresentationImageSelected"/> event.
	/// </summary>
	public class PresentationImageSelectedEventArgs : EventArgs
	{
		private IPresentationImage _selectedPresentationImage;

		internal PresentationImageSelectedEventArgs(
			IPresentationImage selectedPresentationImage)
		{
			Platform.CheckForNullReference(selectedPresentationImage, "selectedPresentationImage");
			_selectedPresentationImage = selectedPresentationImage;
		}

		/// <summary>
		/// Gets the selected <see cref="IPresentationImage"/>.
		/// </summary>
		public IPresentationImage SelectedPresentationImage
		{
			get { return _selectedPresentationImage; }
		}
	}
}
