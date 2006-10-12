using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	internal class TileMemento : IMemento
	{
		private IPresentationImage _presentationImage;

		public TileMemento(IPresentationImage image)
		{
			// image can be null if the Tile is empty, so we don't check for null

			_presentationImage = image;
		}

		public IPresentationImage PresentationImage
		{
			get { return _presentationImage; }
		}

	}
}
