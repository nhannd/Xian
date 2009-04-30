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

#pragma warning disable 1591,0419,1574,1587

using NUnit.Framework;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Tests
{
	[TestFixture]
	public class BuildTreeTest
	{
		IImageViewer _viewer;
		IImageBox _imageBox;
		ITile _tile1;
		ITile _tile2;
		IImageSet _imageSet;
		IDisplaySet _displaySet;
		IPresentationImage _image1;
		IPresentationImage _image2;
	
		private void CreateObjects()
		{
			_viewer = new ImageViewerComponent();
			_imageBox = new ImageBox();
			_tile1 = new Tile();
			_tile2 = new Tile();

			_imageSet = new ImageSet();
			_displaySet = new DisplaySet();
			_image1 = new TestPresentationImage();
			_image2 = new TestPresentationImage();
		}

		public BuildTreeTest()
		{
		}

		[TestFixtureSetUp]
		public void Init()
		{
			Platform.SetExtensionFactory(new NullExtensionFactory());
		}

		[TestFixtureTearDown]
		public void Cleanup()
		{
		}

		[Test]
		public void BuildDownward()
		{
			CreateObjects();
			Assert.AreEqual(_viewer, _viewer.PhysicalWorkspace.ImageViewer);

			// Add image box to physical workspace
			Assert.IsNull(_imageBox.ImageViewer);
			_viewer.PhysicalWorkspace.ImageBoxes.Add(_imageBox);
			Assert.AreEqual(_viewer, _imageBox.ImageViewer);
			Assert.AreEqual(_viewer.PhysicalWorkspace, _imageBox.ParentPhysicalWorkspace);

			// Add tiles to image box
			Assert.IsNull(_tile1.ImageViewer);
			_imageBox.Tiles.Add(_tile1);
			Assert.AreEqual(_viewer, _tile1.ImageViewer);
			Assert.AreEqual(_imageBox, _tile1.ParentImageBox);

			Assert.IsNull(_tile2.ImageViewer);
			_imageBox.Tiles.Add(_tile2);
			Assert.AreEqual(_viewer, _tile2.ImageViewer);
			Assert.AreEqual(_imageBox, _tile2.ParentImageBox);

			// Add image set to logical workspace
			Assert.IsNull(_imageSet.ImageViewer);
			_viewer.LogicalWorkspace.ImageSets.Add(_imageSet);
			Assert.AreEqual(_viewer, _imageSet.ImageViewer);
			Assert.AreEqual(_viewer.LogicalWorkspace, _imageSet.ParentLogicalWorkspace);

			// Add display set to image set
			Assert.IsNull(_displaySet.ImageViewer);
			_imageSet.DisplaySets.Add(_displaySet);
			Assert.AreEqual(_viewer, _displaySet.ImageViewer);
			Assert.AreEqual(_imageSet, _displaySet.ParentImageSet);

			// Add presentation images to display set;
			Assert.IsNull(_image1.ImageViewer);
			_displaySet.PresentationImages.Add(_image1);
			Assert.AreEqual(_viewer, _image1.ImageViewer);
			Assert.AreEqual(_displaySet, _image1.ParentDisplaySet);

			Assert.IsNull(_image2.ImageViewer);
			_displaySet.PresentationImages.Add(_image2);
			Assert.AreEqual(_viewer, _image2.ImageViewer);
			Assert.AreEqual(_displaySet, _image2.ParentDisplaySet);

			// Associate display set with image box
			_imageBox.DisplaySet = _displaySet;
			Assert.AreEqual(_image1, _tile1.PresentationImage);
			Assert.AreEqual(_image2, _tile2.PresentationImage);
		}

		[Test]
		public void BuildUpward()
		{
			CreateObjects();

			// Add tiles to image box
			_imageBox.Tiles.Add(_tile1);
			Assert.IsNull(_tile1.ImageViewer);
			Assert.AreEqual(_imageBox, _tile1.ParentImageBox);

			_imageBox.Tiles.Add(_tile2);
			Assert.IsNull(_tile2.ImageViewer);
			Assert.AreEqual(_imageBox, _tile2.ParentImageBox);

			// Add presentation images to display set
			_displaySet.PresentationImages.Add(_image1);
			Assert.IsNull(_image1.ImageViewer);
			Assert.AreEqual(_displaySet, _image1.ParentDisplaySet);

			_displaySet.PresentationImages.Add(_image2);
			Assert.IsNull(_image2.ImageViewer);
			Assert.AreEqual(_displaySet, _image2.ParentDisplaySet);

			// Associate image box with display set
			_imageBox.DisplaySet = _displaySet;
			Assert.AreEqual(_image1, _tile1.PresentationImage);
			Assert.AreEqual(_image2, _tile2.PresentationImage);

			// Add image box the physical workspace
			_viewer.PhysicalWorkspace.ImageBoxes.Add(_imageBox);

			Assert.AreEqual(_viewer.PhysicalWorkspace, _imageBox.ParentPhysicalWorkspace);

			Assert.AreEqual(_viewer, _imageBox.ImageViewer);
			Assert.AreEqual(_viewer, _tile1.ImageViewer);
			Assert.AreEqual(_viewer, _tile2.ImageViewer);
			Assert.AreEqual(_viewer, _displaySet.ImageViewer);
			Assert.AreEqual(_viewer, _image1.ImageViewer);
			Assert.AreEqual(_viewer, _image2.ImageViewer);
		}

	}
}

#endif