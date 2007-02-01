#if	UNIT_TESTS

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using NUnit.Framework;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.Rendering;

namespace ClearCanvas.ImageViewer.Rendering.Tests
{

/*	[TestFixture]
	public class ImageRendererMiscTests
	{
		public ImageRendererMiscTests()
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
		public void RenderInvalidRectangle()
		{

		}

		[Test]
		public void TestScaleAndRotateTransform()
		{
			Rectangle clientRectangle = new Rectangle(0, 0, 100, 200);
			SpatialTransform transform = new SpatialTransform();
			transform.Rotation = -90;
			transform.Scale = 1.5F;
			transform.ScaleToFit = false;
			transform.SourceRectangle = new Rectangle(0, 0, 100, 200);
			transform.DestinationRectangle = new Rectangle(0, 0, 100, 200);
			transform.Calculate();

			RectangleF srcRectangleF = transform.SourceRectangle;
			RectangleF dstRectangleF = transform.ConvertToDestination(srcRectangleF);

			Assert.IsTrue(FloatComparer.AreEqual(dstRectangleF.Left, -100));
			Assert.IsTrue(FloatComparer.AreEqual(dstRectangleF.Top, 175));
			Assert.IsTrue(FloatComparer.AreEqual(dstRectangleF.Right, 200));
			Assert.IsTrue(FloatComparer.AreEqual(dstRectangleF.Bottom, 25));

			RectangleF dstViewableRectangleF = RectangleUtilities.Intersect(dstRectangleF, clientRectangle);
            RectangleF srcViewableRectangleF = transform.ConvertToSource(dstViewableRectangleF);

			Rectangle dstViewableRectangle = Rectangle.Round(dstViewableRectangleF);

			Assert.AreEqual(0, dstViewableRectangle.Left);
			Assert.AreEqual(175, dstViewableRectangle.Top);
			Assert.AreEqual(100, dstViewableRectangle.Right);
			Assert.AreEqual(25, dstViewableRectangle.Bottom);

			Rectangle srcViewableRectangle = Rectangle.Round(srcViewableRectangleF);

			Assert.AreEqual(0, srcViewableRectangle.Left);
			Assert.AreEqual(67, srcViewableRectangle.Top);
			Assert.AreEqual(100, srcViewableRectangle.Right);
			Assert.AreEqual(134, srcViewableRectangle.Bottom);
		}

		[Test]
		public void TestFlipHorizontalTransform()
		{
			Rectangle clientRectangle = new Rectangle(0, 0, 100, 200);
			SpatialTransform transform = new SpatialTransform();
			transform.FlipHorizontal = true;
			transform.Scale = 1.0F;
			transform.ScaleToFit = false;
			transform.SourceRectangle = new Rectangle(0, 0, 100, 200);
			transform.DestinationRectangle = new Rectangle(0, 0, 100, 200);

			RectangleF srcRectangleF = transform.SourceRectangle;
			RectangleF dstRectangleF = transform.ConvertToDestination(srcRectangleF);
			RectangleF dstViewableRectangleF = RectangleUtilities.Intersect(dstRectangleF, clientRectangle);
			RectangleF srcViewableRectangleF = transform.ConvertToSource(dstViewableRectangleF);

			Rectangle dstViewableRectangle = Rectangle.Round(dstViewableRectangleF);
			Rectangle srcViewableRectangle = Rectangle.Round(srcViewableRectangleF);

			Assert.AreEqual(0, srcViewableRectangle.Left);
			Assert.AreEqual(0, srcViewableRectangle.Top);
			Assert.AreEqual(100, srcViewableRectangle.Right);
			Assert.AreEqual(200, srcViewableRectangle.Bottom);
		}
	}
*/

}
#endif
