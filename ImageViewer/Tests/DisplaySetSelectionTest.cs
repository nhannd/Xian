#if	UNIT_TESTS

using System;
using NUnit.Framework;
using System.Drawing;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.StudyManagement;
using NMock2;

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

			IImageViewer viewer = new ImageViewerComponent("test");
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
			Assert.IsTrue(imageBox[0, 0].Selected);
			Assert.IsFalse(imageBox[0, 1].Selected);

			//Tile tile = imageBox[0, 0] as Tile;
			//Assert.IsTrue(tile.ContextMenuEnabled);
		}


	}
}

#endif