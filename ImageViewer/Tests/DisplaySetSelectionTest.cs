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

namespace ClearCanvas.ImageViewer.Tests
{
	[TestFixture]
	public class DisplaySetSelectionTest
	{
		public DisplaySetSelectionTest()
		{
		}

		[TestFixtureSetUp]
		public void Init()
		{

		}

		[TestFixtureTearDown]
		public void Cleanup()
		{
		}

		[Test]
		public void SelectDisplaySet()
		{
			IDisplaySet displaySet1 = new DisplaySet();
			displaySet1.PresentationImages.Add(new TestPresentationImage());
			displaySet1.PresentationImages.Add(new TestPresentationImage());
			displaySet1.PresentationImages.Add(new TestPresentationImage());
			displaySet1.PresentationImages.Add(new TestPresentationImage());

			IDisplaySet displaySet2 = new DisplaySet();
			displaySet2.PresentationImages.Add(new TestPresentationImage());
			displaySet2.PresentationImages.Add(new TestPresentationImage());

			IImageViewer viewer = new ImageViewerComponent();
			IImageBox imageBox = new ImageBox();
			viewer.PhysicalWorkspace.ImageBoxes.Add(imageBox);

			imageBox.SetTileGrid(2, 2);
			imageBox.DisplaySet = displaySet1;
			imageBox[0, 0].Select();

			Assert.IsTrue(imageBox[0, 0].Selected);
			Assert.IsFalse(imageBox[0, 1].Selected);

			imageBox[0, 1].Select();
			Assert.IsFalse(imageBox[0, 0].Selected);
			Assert.IsTrue(imageBox[0, 1].Selected);

			imageBox.DisplaySet = displaySet2;
			Assert.IsFalse(imageBox[0, 0].Selected);
			Assert.IsTrue(imageBox[0, 1].Selected);
		}

		[Test]
		public void ReplaceDisplaySet()
		{
			IDisplaySet displaySet1 = new DisplaySet();
			IPresentationImage image1 = new TestPresentationImage();
			displaySet1.PresentationImages.Add(image1);

			IDisplaySet displaySet2 = new DisplaySet();
			IPresentationImage image2 = new TestPresentationImage();
			displaySet2.PresentationImages.Add(image2);

			ImageViewerComponent viewer = new ImageViewerComponent();

			IImageBox imageBox1 = new ImageBox();
			viewer.PhysicalWorkspace.ImageBoxes.Add(imageBox1);

			imageBox1.SetTileGrid(2, 2);
			imageBox1.DisplaySet = displaySet1;
			imageBox1[0,0].Select();

			Assert.IsTrue(displaySet1.Selected);
			Assert.IsTrue(image1.Selected);

			imageBox1.DisplaySet = displaySet2;

			Assert.IsFalse(displaySet1.Selected);
			Assert.IsFalse(image1.Selected);

			Assert.IsTrue(displaySet2.Selected);
			Assert.IsTrue(image2.Selected);

		}
	}
}

#endif