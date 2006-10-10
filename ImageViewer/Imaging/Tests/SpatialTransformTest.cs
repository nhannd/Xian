#if	UNIT_TESTS

using System;
using System.Drawing;
//using MbUnit.Core.Framework;
//using MbUnit.Framework;
using NUnit.Framework;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Mathematics;


namespace ClearCanvas.ImageViewer.Imaging.Tests
{
	[TestFixture]
	public class SpatialTransformTest
	{
		public SpatialTransformTest()
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
		public void DefaultTransform()
		{
			SpatialTransform transform = new SpatialTransform();
			transform.SourceRectangle = new Rectangle(0, 0, 512, 384);
			transform.DestinationRectangle = new Rectangle(0, 0, 512, 384);
			transform.Calculate();

			Assert.AreEqual(1.0f, transform.Transform.Elements[0]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[1]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[2]);
			Assert.AreEqual(1.0f, transform.Transform.Elements[3]);
		}

		[Test]
		public void ScaleIsotropicPixel()
		{
			SpatialTransform transform = new SpatialTransform();
			transform.SourceRectangle = new Rectangle(0, 0, 512, 384);
			transform.DestinationRectangle = new Rectangle(0, 0, 512, 384);
			transform.Scale = 2.0f;
			transform.Calculate();

			Assert.AreEqual(2.0f, transform.Transform.Elements[0]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[1]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[2]);
			Assert.AreEqual(2.0f, transform.Transform.Elements[3]);
		}

		[Test]
		public void ScaleAnisotropicPixel()
		{
			SpatialTransform transform = new SpatialTransform();
			transform.SourceRectangle = new Rectangle(0, 0, 512, 384);
			transform.DestinationRectangle = new Rectangle(0, 0, 512, 384);

			// Use pixel aspect ratio
			transform.PixelAspectRatioX = 1;
			transform.PixelAspectRatioY = 2;
			transform.Scale = 2.0f;
			transform.Calculate();

			Assert.AreEqual(4.0f, transform.Transform.Elements[0]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[1]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[2]);
			Assert.AreEqual(2.0f, transform.Transform.Elements[3]);

			transform.Initialize();
			transform.PixelAspectRatioX = 2;
			transform.PixelAspectRatioY = 1;
			transform.Scale = 2.0f;
			transform.Calculate();

			Assert.AreEqual(2.0f, transform.Transform.Elements[0]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[1]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[2]);
			Assert.AreEqual(4.0f, transform.Transform.Elements[3]);

			// Use pixel spacing
			transform.Initialize();
			transform.PixelSpacingX = 0.25f;
			transform.PixelSpacingY = 0.50f;
			transform.Scale = 2.0f;
			transform.Calculate();

			Assert.AreEqual(4.0f, transform.Transform.Elements[0]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[1]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[2]);
			Assert.AreEqual(2.0f, transform.Transform.Elements[3]);

			transform.Initialize();
			transform.PixelSpacingX = 0.50f;
			transform.PixelSpacingY = 0.25f;
			transform.Scale = 2.0f;
			transform.Calculate();

			Assert.AreEqual(2.0f, transform.Transform.Elements[0]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[1]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[2]);
			Assert.AreEqual(4.0f, transform.Transform.Elements[3]);
		}

		[Test]
		public void Flip()
		{
			SpatialTransform transform = new SpatialTransform();
			transform.SourceRectangle = new Rectangle(0, 0, 512, 384);
			transform.DestinationRectangle = new Rectangle(0, 0, 512, 384);

			transform.FlipHorizontal = true;
			transform.Calculate();

			Assert.AreEqual(-1.0f, transform.Transform.Elements[0]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[1]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[2]);
			Assert.AreEqual(1.0f, transform.Transform.Elements[3]);

			transform.Initialize();
			transform.FlipVertical = true;
			transform.Calculate();

			Assert.AreEqual(1.0f, transform.Transform.Elements[0]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[1]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[2]);
			Assert.AreEqual(-1.0f, transform.Transform.Elements[3]);

			transform.Initialize();
			transform.FlipHorizontal = true;
			transform.FlipVertical = true;
			transform.Calculate();

			Assert.AreEqual(-1.0f, transform.Transform.Elements[0]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[1]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[2]);
			Assert.AreEqual(-1.0f, transform.Transform.Elements[3]);
		}

		[Test]
		public void Rotate1()
		{
			SpatialTransform transform = new SpatialTransform();
			transform.SourceRectangle = new Rectangle(0, 0, 512, 384);
			transform.DestinationRectangle = new Rectangle(0, 0, 512, 384);

			transform.Rotation = 90;
			transform.Calculate();
			
			Assert.IsTrue(Math.Abs(0.0f - transform.Transform.Elements[0]) < 1.0E-05);
			Assert.IsTrue(Math.Abs(1.0f - transform.Transform.Elements[1]) < 1.0E-05);
			Assert.IsTrue(Math.Abs(-1.0f - transform.Transform.Elements[2]) < 1.0E-05);
			Assert.IsTrue(Math.Abs(0.0f - transform.Transform.Elements[3]) < 1.0E-05);
		}

		[Test]
		public void Rotate2()
		{
			SpatialTransform transform = new SpatialTransform();

			transform.Rotation = 0;
			Assert.AreEqual(0, transform.Rotation);
			transform.Rotation = 90;
			Assert.AreEqual(90, transform.Rotation);
			transform.Rotation = 360;
			Assert.AreEqual(0, transform.Rotation);
			transform.Rotation = 450;
			Assert.AreEqual(90, transform.Rotation);
			transform.Rotation = -90;
			Assert.AreEqual(270, transform.Rotation);
			transform.Rotation = -270;
			Assert.AreEqual(90, transform.Rotation);
			transform.Rotation = -450;
			Assert.AreEqual(270, transform.Rotation);
		}

		[Test]
		public void Translate()
		{
			SpatialTransform transform = new SpatialTransform();
			transform.SourceRectangle = new Rectangle(0, 0, 512, 384);
			transform.DestinationRectangle = new Rectangle(0, 0, 512, 384);
			transform.TranslationX = 10;
			transform.TranslationY = 20;
			transform.Calculate();

			Assert.AreEqual(1.0f, transform.Transform.Elements[0]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[1]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[2]);
			Assert.AreEqual(1.0f, transform.Transform.Elements[3]);
			Assert.AreEqual(10.0f, transform.Transform.Elements[4]);
			Assert.AreEqual(20.0f, transform.Transform.Elements[5]);
		}

		[Test]
		public void ZeroSizeSourceRectangle()
		{
			Assert.Fail();
		}

		[Test]
		public void ZeroSizeDestinationRectangle()
		{
			Assert.Fail();
		}

		[Test]
		public void ScaleToFit()
		{
			Assert.Fail();
		}

		[Test]
		public void SourceToDestination()
		{
			// be sure to covert back and forth
			SpatialTransform transform = new SpatialTransform();
			transform.SourceRectangle = new Rectangle(0, 0, 3732, 3062);
			//transform.DestinationRectangle = new Rectangle(0, 0, 256, 192);
			transform.DestinationRectangle = new Rectangle(6, 6, 493, 626);
			transform.Calculate();

			PointF srcPt1 = new Point(0, 0);
			PointF dstPt = transform.ConvertToDestination(srcPt1);

			PointF srcPt2 = transform.ConvertToSource(dstPt);
			Assert.IsTrue(FloatComparer.AreEqual(srcPt1, srcPt2));
		}

		[Test]
		public void DestinationToSource()
		{
			Assert.Fail();
		}

		[Test]
		public void OneToOne()
		{
			Assert.Fail();
		}

		[Test]
		public void QuadrantTransformSameImageSize()
		{
			SpatialTransform transform = new SpatialTransform();
			transform.SourceRectangle = new Rectangle(0, 0, 7, 11);
			transform.DestinationRectangle = new Rectangle(0, 0, 7, 11);

			int srcWidth = transform.SourceRectangle.Width;
			int srcHeight = transform.SourceRectangle.Height;
			int dstWidth = transform.DestinationRectangle.Width;
			int dstHeight = transform.DestinationRectangle.Height;
			float oneQuarter = 0.25f;
			float threeQuarters = 0.75f;

			PointF destinationPoint = new PointF((dstWidth * oneQuarter), (dstHeight * oneQuarter));
			PointF sourcePointExpected = new PointF((srcWidth * oneQuarter), (srcHeight * oneQuarter));
			PointF SourcePointCalculated = transform.ConvertToSource(destinationPoint);
			PointF destinationPointBackCalculated = transform.ConvertToDestination(SourcePointCalculated);
			Assert.IsTrue(sourcePointExpected == SourcePointCalculated);
			Assert.IsTrue(destinationPointBackCalculated == destinationPoint);

			transform.FlipHorizontal = true;

			sourcePointExpected.X = (srcWidth * threeQuarters);
			SourcePointCalculated = transform.ConvertToSource(destinationPoint);
			destinationPointBackCalculated = transform.ConvertToDestination(SourcePointCalculated);
			Assert.IsTrue(sourcePointExpected == SourcePointCalculated);
			Assert.IsTrue(destinationPointBackCalculated == destinationPoint);

			transform.FlipHorizontal = false;
			transform.FlipVertical = true;

			sourcePointExpected.X = (srcWidth * oneQuarter);
			sourcePointExpected.Y = (srcHeight * threeQuarters);
			SourcePointCalculated = transform.ConvertToSource(destinationPoint);
			destinationPointBackCalculated = transform.ConvertToDestination(SourcePointCalculated);
			Assert.IsTrue(sourcePointExpected == SourcePointCalculated);
			Assert.IsTrue(destinationPointBackCalculated == destinationPoint);

			transform.FlipHorizontal = true;
			transform.FlipVertical = true;

			sourcePointExpected.X = (srcWidth * threeQuarters);
			sourcePointExpected.Y = (srcHeight * threeQuarters);
			SourcePointCalculated = transform.ConvertToSource(destinationPoint);
			destinationPointBackCalculated = transform.ConvertToDestination(SourcePointCalculated);
			Assert.IsTrue(sourcePointExpected == SourcePointCalculated);
			Assert.IsTrue(destinationPointBackCalculated == destinationPoint);
		}
	}
}

#endif