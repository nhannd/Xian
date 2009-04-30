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
	public class ImageBoxAndDisplaySetInteractionTest
	{
		public ImageBoxAndDisplaySetInteractionTest()
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
		public void SetDisplaySet()
		{
			ImageBox imageBox = new ImageBox();
			IDisplaySet displaySet1 = new DisplaySet();
			IDisplaySet displaySet2 = new DisplaySet();
			PresentationImage image1 = new TestPresentationImage();
			PresentationImage image2 = new TestPresentationImage();
			displaySet1.PresentationImages.Add(image1);
			displaySet2.PresentationImages.Add(image2);

			imageBox.DisplaySet = displaySet1;
			Assert.IsTrue(displaySet1.Visible);
			Assert.AreEqual(imageBox, displaySet1.ImageBox);

			imageBox.DisplaySet = null;
			Assert.IsFalse(displaySet1.Visible);
			Assert.IsNull(displaySet1.ImageBox);

			imageBox.DisplaySet = displaySet1;
			Assert.IsTrue(displaySet1.Visible);
			Assert.AreEqual(imageBox, displaySet1.ImageBox);

			imageBox.DisplaySet = displaySet2;
			Assert.IsTrue(displaySet2.Visible);
			Assert.IsFalse(displaySet1.Visible);
			Assert.AreEqual(imageBox, displaySet2.ImageBox);
			Assert.IsNull(displaySet1.ImageBox);
		}

		[Test]
		public void LayoutImageBoxes()
		{
			IImageViewer viewer = new ImageViewerComponent();
			viewer.PhysicalWorkspace.SetImageBoxGrid(2, 1);

			IDisplaySet displaySet1 = new DisplaySet();
			IDisplaySet displaySet2 = new DisplaySet();
			PresentationImage image1 = new TestPresentationImage();
			PresentationImage image2 = new TestPresentationImage();
			displaySet1.PresentationImages.Add(image1);
			displaySet2.PresentationImages.Add(image2);

			viewer.PhysicalWorkspace.ImageBoxes[0].DisplaySet = displaySet1;
			viewer.PhysicalWorkspace.ImageBoxes[1].DisplaySet = displaySet2;
			viewer.PhysicalWorkspace.SetImageBoxGrid(1, 1);

			Assert.IsFalse(displaySet1.Visible);
			Assert.IsFalse(displaySet2.Visible);
		}
	}
}

#endif