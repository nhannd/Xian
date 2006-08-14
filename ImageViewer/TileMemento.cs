using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Summary description for TileMemento.
	/// </summary>
	internal class TileMemento : IMemento
	{
		private PresentationImage _presentationImage;
		private ClientArea _clientArea;

		public TileMemento(PresentationImage image, ClientArea clientArea)
		{
			Platform.CheckForNullReference(clientArea, "clientArea");

			// image can be null if the Tile is empty, so we don't check for null

			_presentationImage = image;
			_clientArea = clientArea;
		}

		public PresentationImage PresentationImage
		{
			get { return _presentationImage; }
		}

		public ClientArea ClientArea
		{
			get { return _clientArea; }
		}
	}
}
