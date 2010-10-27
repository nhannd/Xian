#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	internal class TileMemento
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
