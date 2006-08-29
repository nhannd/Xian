#if UNIT_TESTS

using System;
using System.Drawing;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Tests
{
	[TestFixture]
	public class PhysicalWorkspaceTest
	{
		LogicalWorkspace _logicalWorkspace;

		public PhysicalWorkspaceTest()
		{
		}

		[TestFixtureSetUp]
		public void Init()
		{
			_logicalWorkspace = new LogicalWorkspace(new ImageViewerComponent("studyUID"));
		}
	
		[TestFixtureTearDown]
		public void Cleanup()
		{
		}

		[Test]
		public void CreateAddGetRemoveImageBox()
		{
			// Create the workspace
			PhysicalWorkspace ws = new PhysicalWorkspace(new ImageViewerComponent("studyUID"));
			Assert.AreEqual(0, ws.ImageBoxes.Count);

			// Add a couple of image boxes
			ImageBox box1 = new ImageBox();
			ws.ImageBoxes.Add(box1);
			Assert.AreEqual(1, ws.ImageBoxes.Count);

			ImageBox box2 = new ImageBox();
			ws.ImageBoxes.Add(box2);
			Assert.AreEqual(2, ws.ImageBoxes.Count);

			// Make sure the two image boxes aren't the same
			Assert.IsTrue(box1 != box2);

			// Make sure the indexing returns what we expect
			Assert.AreEqual(box2, ws.ImageBoxes[1]);

			// Remove an image box.  Make sure we still have left what we think.
			ws.ImageBoxes.Remove(box2);
			Assert.AreEqual(1, ws.ImageBoxes.Count);
			Assert.AreEqual(box1, ws.ImageBoxes[0]);
		}

		[Test]
		public void RectangularGrid()
		{
			PhysicalWorkspace ws = new PhysicalWorkspace(new ImageViewerComponent("studyUID"));
			Assert.AreEqual(0, ws.ImageBoxes.Count);

			ws.ClientRectangle = new Rectangle(0, 0, 100, 200);

			ws.SetImageBoxGrid(1, 2);
			Assert.AreEqual(2, ws.ImageBoxes.Count);

			// Verify tile sizes
			ImageBox box = ws.ImageBoxes[0];
			Assert.AreEqual(new RectangleF(0, 0, 0.5f, 1.0f), box.NormalizedRectangle);
			box = ws.ImageBoxes[1];
			Assert.AreEqual(new RectangleF(0.5f, 0, 0.5f, 1.0f), box.NormalizedRectangle);
			Assert.AreEqual(new Rectangle(50, 0, 50, 200), box.ClientRectangle);

			ws.SetImageBoxGrid(4, 3);
			Assert.AreEqual(12, ws.ImageBoxes.Count);
		}

		[Test]
		public void ClientArea()
		{
			PhysicalWorkspace ws = new PhysicalWorkspace(new ImageViewerComponent("studyUID"));

			// Test for initial values
			Rectangle rect0 = new Rectangle(0, 0, 0, 0);
			Assert.AreEqual(rect0, ws.ClientRectangle);
		}

		[Test]
		public void CallSequence()
		{
			// Create workspace
			PhysicalWorkspace ws = new PhysicalWorkspace(new ImageViewerComponent("studyUID"));
			Assert.AreEqual(0, ws.ImageBoxes.Count);

			// Set the workspace size
			ws.ClientRectangle = new Rectangle(0, 0, 100, 200);

			// Create the image box, set its normalized size and add it to the workspace
			ImageBox box = new ImageBox();
			box.NormalizedRectangle = new RectangleF(0, 0, 0.50f, 1.0f);
			ws.ImageBoxes.Add(box);

			// Verify that the image box has the right client size.
			box = ws.ImageBoxes[0];
			Assert.AreEqual(new Rectangle(0, 0, 50, 200), box.ClientRectangle);

			// Change the workspace's parent rectangle and make sure
			// the image box changes with it
			ws.ClientRectangle = new Rectangle(0, 0, 50, 100);
			Assert.AreEqual(new Rectangle(0, 0, 25, 100), box.ClientRectangle);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void AddNullImageBox()
		{
			PhysicalWorkspace ws = new PhysicalWorkspace(new ImageViewerComponent("studyUID"));
			ws.ImageBoxes.Add(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void RemoveNullImageBox()
		{
			PhysicalWorkspace ws = new PhysicalWorkspace(new ImageViewerComponent("studyUID"));
			ws.ImageBoxes.Remove(null);
		}

		[Test]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void IndexOutOfRangeTooLow()
		{
			PhysicalWorkspace ws = new PhysicalWorkspace(new ImageViewerComponent("studyUID"));
			Assert.AreEqual(0, ws.ImageBoxes.Count);

			ImageBox box1 = new ImageBox();
			ws.ImageBoxes.Add(box1);
			Assert.AreEqual(1, ws.ImageBoxes.Count);

			ImageBox box2 = ws.ImageBoxes[-1];
		}

		[Test]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void IndexOutOfRangeTooHigh()
		{
			PhysicalWorkspace ws = new PhysicalWorkspace(new ImageViewerComponent("studyUID"));
			Assert.AreEqual(0, ws.ImageBoxes.Count);

			ImageBox box1 = new ImageBox();
			ws.ImageBoxes.Add(box1);
			Assert.AreEqual(1, ws.ImageBoxes.Count);

			ImageBox box2 = ws.ImageBoxes[1];
		}

		[Test]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void IndexOutOfRangeEmptyWorkspace()
		{
			PhysicalWorkspace ws = new PhysicalWorkspace(new ImageViewerComponent("studyUID"));
			Assert.AreEqual(0, ws.ImageBoxes.Count);

			ImageBox box2 = ws.ImageBoxes[0];
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void InvalidGridSize()
		{
			PhysicalWorkspace ws = new PhysicalWorkspace(new ImageViewerComponent("studyUID"));
			ws.SetImageBoxGrid(-1, 0);
		}
	}
}

#endif
