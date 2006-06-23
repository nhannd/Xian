using System;
using ClearCanvas.Common;

namespace ClearCanvas.Workstation.Model
{
	public class ImageDrawingEventArgs : EventArgs
	{
		// Private attributes
		private PresentationImage m_PresentationImage;
		private PhysicalWorkspace m_PhysicalWorkspace;
		private ImageBox m_ImageBox;
		private Tile m_Tile;
		private bool m_ImageBoxLayoutChanged = false;
		private bool m_TileLayoutChanged = false;
		private bool m_PaintNow = false;

		// Constructor
		public ImageDrawingEventArgs(PresentationImage presentationImage, 
			bool paintNow)
		{
			//Platform.CheckForNullReference(presentationImage, "presentationImage");

			m_PresentationImage = presentationImage;
			m_PaintNow = paintNow;
		}

		// Properties
		public PresentationImage PresentationImage 
		{ 
			get { return m_PresentationImage; } 
		}

		public PhysicalWorkspace PhysicalWorkspace
		{ 
			get { return m_PhysicalWorkspace; } 
			set 
			{
				Platform.CheckForNullReference(value, "PhysicalWorkapce");
				m_PhysicalWorkspace = value; 
			}
		}

		public ImageBox ImageBox
		{ 
			get { return m_ImageBox; } 
			set 
			{ 
				Platform.CheckForNullReference(value, "ImageBox");
				m_ImageBox = value; 
			}
		}

		public Tile Tile
		{ 
			get { return m_Tile; } 
			set 
			{ 
				Platform.CheckForNullReference(value, "Tile");
				m_Tile = value; 
			}
		}

		public bool ImageBoxLayoutChanged
		{
			get { return m_ImageBoxLayoutChanged; }
			set { m_ImageBoxLayoutChanged = value; }
		}

		public bool TileLayoutChanged
		{
			get { return m_TileLayoutChanged; }
			set { m_TileLayoutChanged = value; }
		}

		public bool PaintNow
		{
			get { return m_PaintNow; }
			set { m_PaintNow = value; }
		}
	}
}
