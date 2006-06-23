#if UNIT_TESTS

using System;
using System.Drawing;
using NUnit.Framework;

namespace ClearCanvas.Workstation.Model.Tests
{
	[TestFixture]
	public class ImageBoxTest
	{
		public ImageBoxTest()
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
		public void CreateAddGetRemoveTile()
		{
			// Create the image box
			ImageBox box = new ImageBox();
			Assert.AreEqual(0, box.Tiles.Count);

			// Add a couple of tiles
/*			Tile tile1 = new Tile();
			box.AddTile(tile1);
			Assert.AreEqual(1, box.Tiles.Count);

			Tile tile2 = new Tile();
			box.AddTile(tile2);
			Assert.AreEqual(2, box.Tiles.Count);

			// Make sure the two tiles aren't the same
			Assert.IsTrue(tile1 != tile2);

			// Make sure the indexing returns what we expect
			Assert.AreEqual(tile2, box[1]);

			// Remove a tile.  Make sure we still have left what we think.
			box.RemoveTile(tile2);
			Assert.AreEqual(1, box.Tiles.Count);
			Assert.AreEqual(tile1, box[0]);
*/
		}

		[Test]
		public void RectangularGrid()
		{
			// Create the image box
			ImageBox box = new ImageBox();
			Assert.IsTrue(box.Tiles.Count == 0);

			box.NormalizedRectangle = new RectangleF(0.0f, 0.0f, 1.0f, 1.0f);
			box.ParentRectangle = new Rectangle(0, 0, 100, 200);

			// Verify number of tiles
			box.SetTileGrid(1,2);
			Assert.AreEqual(2, box.Tiles.Count);

			// Verify tile sizes
			Tile tile = box.Tiles[0];
			Assert.AreEqual(new RectangleF(0, 0, 0.5f, 1.0f), tile.NormalizedRectangle);
			tile = box.Tiles[1];
			Assert.AreEqual(new RectangleF(0.5f, 0, 0.5f, 1.0f), tile.NormalizedRectangle);
			Assert.AreEqual(new Rectangle(52, 2, 46, 196), tile.ClientRectangle);

			// Change grid and verify number of tiles again
			box.SetTileGrid(4,3);
			Assert.AreEqual(12, box.Tiles.Count);
		}

		[Test]
		public void ClientArea()
		{
			ImageBox box = new ImageBox();

			// Test for initial values
			Rectangle rect0 = new Rectangle(0, 0, 0, 0);
			Assert.IsTrue(box.ClientRectangle == rect0);
			Assert.IsTrue(box.ParentRectangle == rect0);

			RectangleF rect1 = new RectangleF(0.0f, 0.0f, 0.0f, 0.0f);
			Assert.IsTrue(box.NormalizedRectangle == rect1);

			// Set the normalized rectangle
			RectangleF rect2 = new RectangleF(0.25f, 0.25f, 0.50f, 0.50f);
			box.NormalizedRectangle = rect2;
			Assert.IsTrue(box.NormalizedRectangle == rect2);

			// Client rectangle should be computed based on the normalized
			// and parent rectangles
			Rectangle rect3 = new Rectangle(100, 100, 200, 200);
			box.ParentRectangle = rect3;
			Assert.IsTrue(box.ClientRectangle == new Rectangle(150, 150, 100, 100));
		}

		[Test]
		public void DisplaySet()
		{
			ImageBox box = new ImageBox();
			DisplaySet ds1 = new DisplaySet();
			box.DisplaySet = ds1;
			Assert.AreEqual(ds1, box.DisplaySet);
			//Assert.IsTrue(((IBooleanState)ds1).Visible);

			DisplaySet ds2 = new DisplaySet();
			box.DisplaySet = ds2;
			//Assert.IsFalse(((IBooleanState)ds1).Visible);
			//Assert.IsTrue(((IBooleanState)ds2).Visible);
		}

		[Test]
		public void CallSequence()
		{
			// Create the image box
			ImageBox box = new ImageBox();
			Assert.IsTrue(box.Tiles.Count == 0);

			// Set the image box size
			box.NormalizedRectangle = new RectangleF(0.0f, 0.0f, 1.0f, 1.0f);
			box.ParentRectangle = new Rectangle(0, 0, 100, 200);

			// Create the tile, set its normalized size and add it to the image box
			Tile tile = new Tile();
			tile.NormalizedRectangle = new RectangleF(0, 0, 0.50f, 1.0f);
			//box.AddTile(tile);

			// Verify that the tile has the right client size.  (We inflate the rect
			// because it comes to us deflated...easier numbers to read when inflated.)
			tile = box.Tiles[0];
			Rectangle rect = Rectangle.Inflate(tile.ClientRectangle, 2, 2);
			Assert.AreEqual(new Rectangle(0, 0, 50, 200), rect);

			// Change the image box's parent rectangle and make sure
			// the tile changes with it
			box.ParentRectangle = new Rectangle(0, 0, 50, 100);
			Assert.AreEqual(new Rectangle(0, 0, 50, 100), box.ClientRectangle);
			rect = Rectangle.Inflate(tile.ClientRectangle, 2, 2);
			Assert.AreEqual(new Rectangle(0, 0, 25, 100), rect);

			// Change the image box's normalized rectangle and make sure
			// the tile changes with it
			box.NormalizedRectangle = new RectangleF(0, 0, 0.8f, 0.5f);
			Assert.AreEqual(new Rectangle(0, 0, 40, 50), box.ClientRectangle);
			rect = Rectangle.Inflate(tile.ClientRectangle, 2, 2);
			Assert.AreEqual(new Rectangle(0, 0, 20, 50), rect);
		}

		/*
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void AddNullTile()
		{
			ImageBox box = new ImageBox();
			box.AddTile(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void RemoveNullTile()
		{
			ImageBox box = new ImageBox();
			box.RemoveTile(null);
		}
		*/

		[Test]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void IndexOutOfRangeTooLow()
		{
			ImageBox box = new ImageBox();
			Assert.IsTrue(box.Tiles.Count == 0);

			Tile tile1 = new Tile();
			//box.AddTile(tile1);
			Assert.IsTrue(box.Tiles.Count == 1);

			Tile tile2 = box.Tiles[-1];
		}

		[Test]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void IndexOutOfRangeTooHigh()
		{
			ImageBox box = new ImageBox();
			Assert.IsTrue(box.Tiles.Count == 0);

			Tile tile1 = new Tile();
			//box.AddTile(tile1);
			Assert.IsTrue(box.Tiles.Count == 1);

			Tile tile2 = box.Tiles[1];
		}

		[Test]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void IndexOutOfRangeEmptyImageBox()
		{
			ImageBox box = new ImageBox();
			Assert.IsTrue(box.Tiles.Count == 0);

			Tile tile2 = box.Tiles[0];
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void InvalidGridSize()
		{
			ImageBox box = new ImageBox();
			box.SetTileGrid(0,-1);
		}
	}
}

#endif