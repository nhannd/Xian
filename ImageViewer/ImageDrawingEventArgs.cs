using System;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	public class ImageDrawingEventArgs : EventArgs
	{
		// Private attributes
		private PresentationImage _PresentationImage;
		private PhysicalWorkspace _PhysicalWorkspace;
		private ImageBox _ImageBox;
		private Tile _Tile;
		private bool _ImageBoxLayoutChanged = false;
		private bool _TileLayoutChanged = false;
		private bool _PaintNow = false;

		// Constructor
		public ImageDrawingEventArgs(PresentationImage presentationImage, 
			bool paintNow)
		{
			//Platform.CheckForNullReference(presentationImage, "presentationImage");

			_PresentationImage = presentationImage;
			_PaintNow = paintNow;
		}

		// Properties
		public PresentationImage PresentationImage 
		{ 
			get { return _PresentationImage; } 
		}

		public PhysicalWorkspace PhysicalWorkspace
		{ 
			get { return _PhysicalWorkspace; } 
			set 
			{
				Platform.CheckForNullReference(value, "PhysicalWorkapce");
				_PhysicalWorkspace = value; 
			}
		}

		public ImageBox ImageBox
		{ 
			get { return _ImageBox; } 
			set 
			{ 
				Platform.CheckForNullReference(value, "ImageBox");
				_ImageBox = value; 
			}
		}

		public Tile Tile
		{ 
			get { return _Tile; } 
			set 
			{ 
				Platform.CheckForNullReference(value, "Tile");
				_Tile = value; 
			}
		}

		public bool ImageBoxLayoutChanged
		{
			get { return _ImageBoxLayoutChanged; }
			set { _ImageBoxLayoutChanged = value; }
		}

		public bool TileLayoutChanged
		{
			get { return _TileLayoutChanged; }
			set { _TileLayoutChanged = value; }
		}

		public bool PaintNow
		{
			get { return _PaintNow; }
			set { _PaintNow = value; }
		}
	}
}
