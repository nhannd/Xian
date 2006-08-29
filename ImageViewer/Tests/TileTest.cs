#if UNIT_TESTS

using System;
using System.Drawing;
using NUnit.Framework;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Tests
{
	[TestFixture]
	public class TileTest
	{
		LogicalWorkspace _workspace;
		DisplaySet _displaySet;

		ImageDrawingEventArgs _drawImageEventArgs;

		public TileTest()
		{
		}

		[TestFixtureSetUp]
		public void Init()
		{
			_workspace = new LogicalWorkspace(new ImageViewerComponent("studyUID"));
			_displaySet = new DisplaySet();
		}
	
		[TestFixtureTearDown]
		public void Cleanup()
		{
		}

		[Test]
		public void PresentationImage()
		{
			Tile tile = new Tile();
			PresentationImage pi1 = new DicomPresentationImage(null);
			Assert.IsFalse(pi1.Visible);
			
			tile.PresentationImage = pi1;
			Assert.AreEqual(pi1, tile.PresentationImage);
			Assert.IsTrue(pi1.Visible);
			
			PresentationImage pi2 = new DicomPresentationImage(null);
			Assert.IsFalse(pi2.Visible);
			
			tile.PresentationImage = pi2;
			Assert.AreEqual(pi2, tile.PresentationImage);
			Assert.IsTrue(pi2.Visible);
			Assert.IsFalse(pi1.Visible);
		}

		[Test]
		public void DrawImage()
		{
			Tile tile = new Tile();
			tile.ParentRectangle = new Rectangle(0,0,100,200);
			tile.NormalizedRectangle = new RectangleF(0,0,0.5f,1.0f);
			PresentationImage pi1 = new DicomPresentationImage(null);
			tile.PresentationImage = pi1;
			//pi1.ImageProcessor = new ImageProcessor2D();

			tile.ImageDrawing += new EventHandler<ImageDrawingEventArgs>(OnDrawImage);

			pi1.Draw(false);
			Assert.AreEqual(new Rectangle(0,0,50,100), _drawImageEventArgs.Tile.ClientRectangle);
		}

		private void OnDrawImage(object sender, EventArgs e)
		{
			_drawImageEventArgs = (ImageDrawingEventArgs) e;
		}
	}
}

#endif
