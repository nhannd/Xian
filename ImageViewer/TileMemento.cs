using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	internal class TileMemento : IMemento
	{
		private PresentationImage _presentationImage;

		public TileMemento(PresentationImage image)
		{
			// image can be null if the Tile is empty, so we don't check for null

			_presentationImage = image;
		}

		public PresentationImage PresentationImage
		{
			get { return _presentationImage; }
		}

	}
}
