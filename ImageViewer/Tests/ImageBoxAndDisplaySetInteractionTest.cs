#if	UNIT_TESTS

using System;
using NUnit.Framework;
using System.Drawing;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.StudyManagement;

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
		public void ImageBox_DisplaySet()
		{
			ImageBox imageBox = new ImageBox();
			DisplaySet displaySet1 = new DisplaySet();
			DisplaySet displaySet2 = new DisplaySet();
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


	}
}

#endif