using System;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	public class ImageDrawingEventArgs : EventArgs
	{
		// Private attributes
		private PresentationImage _presentationImage;
		private PhysicalWorkspace _physicalWorkspace;
		private ImageBox _imageBox;
		private Tile _tile;
		private bool _imageBoxLayoutChanged = false;
		private bool _tileLayoutChanged = false;
		private bool _paintNow = false;
		private bool _fastDraw = false;

		// Constructor
		public ImageDrawingEventArgs(PresentationImage presentationImage, 
			bool paintNow)
		{
			//Platform.CheckForNullReference(presentationImage, "presentationImage");

			_presentationImage = presentationImage;
			_paintNow = paintNow;
		}

		// Properties
		public PresentationImage PresentationImage 
		{ 
			get { return _presentationImage; } 
		}

		public PhysicalWorkspace PhysicalWorkspace
		{ 
			get { return _physicalWorkspace; } 
			set 
			{
				Platform.CheckForNullReference(value, "PhysicalWorkapce");
				_physicalWorkspace = value; 
			}
		}

		public ImageBox ImageBox
		{ 
			get { return _imageBox; } 
			set 
			{ 
				Platform.CheckForNullReference(value, "ImageBox");
				_imageBox = value; 
			}
		}

		public Tile Tile
		{ 
			get { return _tile; } 
			set 
			{ 
				Platform.CheckForNullReference(value, "Tile");
				_tile = value; 
			}
		}

		public bool ImageBoxLayoutChanged
		{
			get { return _imageBoxLayoutChanged; }
			set { _imageBoxLayoutChanged = value; }
		}

		public bool TileLayoutChanged
		{
			get { return _tileLayoutChanged; }
			set { _tileLayoutChanged = value; }
		}

		public bool PaintNow
		{
			get { return _paintNow; }
			set { _paintNow = value; }
		}

		public bool FastDraw
		{
			get { return _fastDraw; }
			set { _fastDraw = value; }
		}
	}
}
