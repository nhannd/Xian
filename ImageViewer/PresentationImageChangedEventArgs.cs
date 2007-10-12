using System;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Provides data for the <see cref="Tile.PresentationImageChanged"/> event.
	/// </summary>
	public class PresentationImageChangedEventArgs : EventArgs
	{
		private IPresentationImage _oldPresentationImage;
		private IPresentationImage _newPresentationImage;

		/// <summary>
		/// Initializes a new instance of <see cref="PresentationImageChangedEventArgs"/>.
		/// </summary>
		/// <param name="oldPresentationImage"></param>
		/// <param name="newPresentationImage"></param>
		public PresentationImageChangedEventArgs(
			IPresentationImage oldPresentationImage,
			IPresentationImage newPresentationImage)
		{
			_oldPresentationImage = oldPresentationImage;
			_newPresentationImage = newPresentationImage;
		}

		/// <summary>
		/// Gets the old <see cref="IPresentationImage"/>.
		/// </summary>
		public IPresentationImage OldPresentationImage
		{
			get { return _oldPresentationImage; }
		}

		/// <summary>
		/// Gets the new <see cref="IPresentationImage"/>.
		/// </summary>
		public IPresentationImage NewPresentationImage
		{
			get { return _newPresentationImage; }
		}
	}
}
