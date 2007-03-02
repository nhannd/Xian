#if	UNIT_TESTS

#pragma warning disable 1591,0419,1574,1587

using System;
using NUnit.Framework;
using System.Drawing;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.StudyManagement;
using NMock2;

namespace ClearCanvas.ImageViewer.Tests
{
	[TestFixture]
	public class SelectionTest
	{
		public SelectionTest()
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
		public void SelectTilesInSameImageBox()
		{
			TestTree testTree = new TestTree();

			testTree.Tile1.Select();
			Assert.AreEqual(testTree.Tile1, testTree.ImageBox1.SelectedTile);
			Assert.AreEqual(testTree.ImageBox1, testTree.Viewer.PhysicalWorkspace.SelectedImageBox);
			Assert.IsTrue(testTree.Image1.Selected);
			Assert.IsTrue(testTree.Tile1.Selected);
			Assert.IsTrue(testTree.DisplaySet1.Selected);
			Assert.IsTrue(testTree.ImageBox1.Selected);

			Assert.IsFalse(testTree.Image2.Selected);
			Assert.IsFalse(testTree.Image3.Selected);
			Assert.IsFalse(testTree.Image4.Selected);
			Assert.IsFalse(testTree.Tile2.Selected);
			Assert.IsFalse(testTree.Tile3.Selected);
			Assert.IsFalse(testTree.Tile4.Selected);

			Assert.IsFalse(testTree.DisplaySet2.Selected);
			Assert.IsFalse(testTree.ImageBox2.Selected);

			testTree.Tile2.Select();

			Assert.AreEqual(testTree.Tile2, testTree.ImageBox1.SelectedTile);
			Assert.AreEqual(testTree.ImageBox1, testTree.Viewer.PhysicalWorkspace.SelectedImageBox);
			Assert.IsTrue(testTree.Image2.Selected);
			Assert.IsTrue(testTree.Tile2.Selected);
			Assert.IsTrue(testTree.DisplaySet1.Selected);
			Assert.IsTrue(testTree.ImageBox1.Selected);

			Assert.IsFalse(testTree.Image1.Selected);
			Assert.IsFalse(testTree.Image3.Selected);
			Assert.IsFalse(testTree.Image4.Selected);
			Assert.IsFalse(testTree.Tile1.Selected);
			Assert.IsFalse(testTree.Tile3.Selected);
			Assert.IsFalse(testTree.Tile4.Selected);

			Assert.IsFalse(testTree.DisplaySet2.Selected);
			Assert.IsFalse(testTree.ImageBox2.Selected);
		}

		[Test]
		public void SelectTilesInDiffferentImageBoxes()
		{
			TestTree testTree = new TestTree();

			testTree.Tile1.Select();
			Assert.AreEqual(testTree.Tile1, testTree.ImageBox1.SelectedTile);
			Assert.AreEqual(testTree.ImageBox1, testTree.Viewer.PhysicalWorkspace.SelectedImageBox);
			Assert.IsTrue(testTree.Image1.Selected);
			Assert.IsTrue(testTree.Tile1.Selected);
			Assert.IsTrue(testTree.DisplaySet1.Selected);
			Assert.IsTrue(testTree.ImageBox1.Selected);

			Assert.IsFalse(testTree.Image2.Selected);
			Assert.IsFalse(testTree.Image3.Selected);
			Assert.IsFalse(testTree.Image4.Selected);
			Assert.IsFalse(testTree.Tile2.Selected);
			Assert.IsFalse(testTree.Tile3.Selected);
			Assert.IsFalse(testTree.Tile4.Selected);

			Assert.IsFalse(testTree.DisplaySet2.Selected);
			Assert.IsFalse(testTree.ImageBox2.Selected);

			testTree.Tile3.Select();

			Assert.AreEqual(testTree.Tile3, testTree.ImageBox2.SelectedTile);
			Assert.AreEqual(testTree.ImageBox2, testTree.Viewer.PhysicalWorkspace.SelectedImageBox);
			Assert.IsTrue(testTree.Image3.Selected);
			Assert.IsTrue(testTree.Tile3.Selected);
			Assert.IsTrue(testTree.DisplaySet2.Selected);
			Assert.IsTrue(testTree.ImageBox2.Selected);

			Assert.IsFalse(testTree.Image1.Selected);
			Assert.IsFalse(testTree.Image2.Selected);
			Assert.IsFalse(testTree.Image4.Selected);
			Assert.IsFalse(testTree.Tile1.Selected);
			Assert.IsFalse(testTree.Tile2.Selected);
			Assert.IsFalse(testTree.Tile4.Selected);

			Assert.IsFalse(testTree.DisplaySet1.Selected);
			Assert.IsFalse(testTree.ImageBox1.Selected);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void SelectTileBeforeAddingToTree()
		{
			ITile tile = new Tile();
			tile.Select();
		}
	}
}

#endif