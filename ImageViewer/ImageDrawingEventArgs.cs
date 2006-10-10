using System;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	public class ImageDrawingEventArgs : EventArgs
	{
		// Private attributes
		private IPresentationImage _presentationImage;

		// Constructor
		public ImageDrawingEventArgs(IPresentationImage presentationImage)
		{
			Platform.CheckForNullReference(presentationImage, "presentationImage");

			_presentationImage = presentationImage;
		}

		// Properties
		public IPresentationImage PresentationImage 
		{ 
			get { return _presentationImage; } 
		}
	}
}
