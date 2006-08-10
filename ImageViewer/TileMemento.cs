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
		private PresentationImage _PresentationImage;
		private ClientArea _ClientArea;

		public TileMemento(PresentationImage image, ClientArea clientArea)
		{
			Platform.CheckForNullReference(clientArea, "clientArea");

			// image can be null if the Tile is empty, so we don't check for null

			_PresentationImage = image;
			_ClientArea = clientArea;
		}

		public PresentationImage PresentationImage
		{
			get { return _PresentationImage; }
		}

		public ClientArea ClientArea
		{
			get { return _ClientArea; }
		}
	}
}
