#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

#if	UNIT_TESTS

using ClearCanvas.ImageViewer.Rendering;

namespace ClearCanvas.ImageViewer.Tests
{
	class TestPresentationImage : PresentationImage
	{
		public override IRenderer ImageRenderer
		{
			get { return null; }
		}

		public override IPresentationImage CreateFreshCopy()
		{
			return new TestPresentationImage();
		}
	}

	class TestTree
	{
		IImageViewer _viewer;
		IImageBox _imageBox1;
		IImageBox _imageBox2;
		ITile _tile1;
		ITile _tile2;
		ITile _tile3;
		ITile _tile4;
		IImageSet _imageSet1;
		IDisplaySet _displaySet1;
		IDisplaySet _displaySet2;
		IPresentationImage _image1;
		IPresentationImage _image2;
		IPresentationImage _image3;
		IPresentationImage _image4;

		public TestTree()
		{
			_viewer = new ImageViewerComponent();
			
			_imageBox1 = new ImageBox();
			_imageBox2 = new ImageBox();
			
			_tile1 = new Tile();
			_tile2 = new Tile();
			_tile3 = new Tile();
			_tile4 = new Tile();

			_imageSet1 = new ImageSet();

			_displaySet1 = new DisplaySet();
			_displaySet2 = new DisplaySet();

			_image1 = new TestPresentationImage();
			_image2 = new TestPresentationImage();
			_image3 = new TestPresentationImage();
			_image4 = new TestPresentationImage();

			_viewer.PhysicalWorkspace.ImageBoxes.Add(_imageBox1);
			_viewer.PhysicalWorkspace.ImageBoxes.Add(_imageBox2);

			_imageBox1.Tiles.Add(_tile1);
			_imageBox1.Tiles.Add(_tile2);
			_imageBox2.Tiles.Add(_tile3);
			_imageBox2.Tiles.Add(_tile4);

			_viewer.LogicalWorkspace.ImageSets.Add(_imageSet1);

			_imageSet1.DisplaySets.Add(_displaySet1);
			_imageSet1.DisplaySets.Add(_displaySet2);
			
			_displaySet1.PresentationImages.Add(_image1);
			_displaySet1.PresentationImages.Add(_image2);
			_displaySet2.PresentationImages.Add(_image3);
			_displaySet2.PresentationImages.Add(_image4);
			
			_imageBox1.DisplaySet = _displaySet1;
			_imageBox2.DisplaySet = _displaySet2;
		}

		public IImageViewer Viewer { get { return _viewer; } }
		public IImageBox ImageBox1 { get { return _imageBox1; } }
		public IImageBox ImageBox2 { get { return _imageBox2; } }
		public ITile Tile1 { get { return _tile1; } }
		public ITile Tile2 { get { return _tile2; } }
		public ITile Tile3 { get { return _tile3; } }
		public ITile Tile4 { get { return _tile4; } }
		public IImageSet ImageSet1 { get { return _imageSet1; } }
		public IDisplaySet DisplaySet1 { get { return _displaySet1; } }
		public IDisplaySet DisplaySet2 { get { return _displaySet2; } }
		public IPresentationImage Image1 { get { return _image1; } }
		public IPresentationImage Image2 { get { return _image2; } }
		public IPresentationImage Image3 { get { return _image3; } }
		public IPresentationImage Image4 { get { return _image4; } }

	}
}

#endif