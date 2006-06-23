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
		private PresentationImage m_PresentationImage;
		private ClientArea m_ClientArea;

		public TileMemento(PresentationImage image, ClientArea clientArea)
		{
			Platform.CheckForNullReference(clientArea, "clientArea");

			// image can be null if the Tile is empty, so we don't check for null

			m_PresentationImage = image;
			m_ClientArea = clientArea;
		}

		public PresentationImage PresentationImage
		{
			get { return m_PresentationImage; }
		}

		public ClientArea ClientArea
		{
			get { return m_ClientArea; }
		}
	}
}
