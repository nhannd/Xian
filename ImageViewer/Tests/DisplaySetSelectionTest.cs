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

			IImageViewer viewer = new MockImageViewerComponent();
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

		[Test]
		public void ReplaceDisplaySet()
		{
			IDisplaySet displaySet1 = new DisplaySet();
			IPresentationImage image1 = new TestPresentationImage();
			displaySet1.PresentationImages.Add(image1);

			IDisplaySet displaySet2 = new DisplaySet();
			IPresentationImage image2 = new TestPresentationImage();
			displaySet2.PresentationImages.Add(image2);

			IImageViewer viewer = new DiagnosticImageViewerComponent();

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