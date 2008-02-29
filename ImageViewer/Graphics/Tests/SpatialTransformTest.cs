#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Drawing;
using System;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Graphics.Tests
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
			ImageSpatialTransform transform = CreateTransform();
			transform.ScaleToFit = false;

			Assert.AreEqual(1.0f, transform.Transform.Elements[0]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[1]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[2]);
			Assert.AreEqual(1.0f, transform.Transform.Elements[3]);
		}

		[Test]
		public void Scale()
		{
			ImageSpatialTransform transform = CreateTransform();
			transform.ScaleToFit = false;
			transform.Scale = 2.0f;

			Assert.AreEqual(transform.ScaleX, 2.0f);
			Assert.AreEqual(transform.ScaleY, 2.0f);
		}

		[Test]
		public void ScaleIsotropicPixel()
		{
			ImageSpatialTransform transform = CreateTransform();
			transform.ScaleToFit = false;
			transform.Scale = 2.0f;

			Assert.AreEqual(2.0f, transform.Transform.Elements[0]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[1]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[2]);
			Assert.AreEqual(2.0f, transform.Transform.Elements[3]);
		}


		[Test]
		public void ScaleAnisotropicPixelSpacing1()
		{
			CompositeImageGraphic graphic = new CompositeImageGraphic(512, 384, 1, 2, 0, 0);
			ImageSpatialTransform transform = (ImageSpatialTransform)graphic.SpatialTransform;
			transform.ClientRectangle = new Rectangle(0, 0, 384, 512);

			transform.ScaleToFit = false;
			transform.Scale = 2.0f;

			Assert.AreEqual(2.0f, transform.Transform.Elements[0]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[1]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[2]);
			Assert.AreEqual(4.0f, transform.Transform.Elements[3]);
		}

		[Test]
		public void ScaleAnisotropicPixelSpacing2()
		{
			CompositeImageGraphic graphic = new CompositeImageGraphic(512, 384, 2, 1, 0, 0);
			ImageSpatialTransform transform = (ImageSpatialTransform)graphic.SpatialTransform;
			transform.ClientRectangle = new Rectangle(0, 0, 384, 512);

			transform.ScaleToFit = false;
			transform.Scale = 2.0f;

			Assert.AreEqual(4.0f, transform.Transform.Elements[0]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[1]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[2]);
			Assert.AreEqual(2.0f, transform.Transform.Elements[3]);
		}

		[Test]
		public void ScaleAnisotropicPixelAspectRatio1()
		{
			CompositeImageGraphic graphic = new CompositeImageGraphic(512, 384, 0, 0, 1, 2);
			ImageSpatialTransform transform = (ImageSpatialTransform)graphic.SpatialTransform;
			transform.ClientRectangle = new Rectangle(0, 0, 384, 512);

			transform.ScaleToFit = false;
			transform.Scale = 2.0f;

			Assert.AreEqual(2.0f, transform.Transform.Elements[0]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[1]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[2]);
			Assert.AreEqual(4.0f, transform.Transform.Elements[3]);
		}

		[Test]
		public void ScaleAnisotropicPixelAspectRatio2()
		{
			CompositeImageGraphic graphic = new CompositeImageGraphic(512, 384, 0, 0, 2, 1);
			ImageSpatialTransform transform = (ImageSpatialTransform)graphic.SpatialTransform;
			transform.ClientRectangle = new Rectangle(0, 0, 384, 512);

			transform.ScaleToFit = false;
			transform.Scale = 2.0f;

			Assert.AreEqual(4.0f, transform.Transform.Elements[0]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[1]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[2]);
			Assert.AreEqual(2.0f, transform.Transform.Elements[3]);
		}

		[Test]
		public void FlipY()
		{
			ImageSpatialTransform transform = CreateTransform();
			transform.FlipY = true;

			Assert.AreEqual(-1.0f, transform.Transform.Elements[0]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[1]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[2]);
			Assert.AreEqual(1.0f, transform.Transform.Elements[3]);
			Assert.AreEqual(transform.Scale, 1.0f);
			Assert.AreEqual(transform.ScaleX, 1.0f);
			Assert.AreEqual(transform.ScaleY, 1.0f);
		}

		[Test]
		public void FlipX()
		{
			ImageSpatialTransform transform = CreateTransform();
			transform.ScaleToFit = false;
			transform.FlipX = true;

			Assert.AreEqual(1.0f, transform.Transform.Elements[0]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[1]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[2]);
			Assert.AreEqual(-1.0f, transform.Transform.Elements[3]);
			Assert.AreEqual(transform.Scale, 1.0f);
			Assert.AreEqual(transform.ScaleX, 1.0f);
			Assert.AreEqual(transform.ScaleY, 1.0f);
		}

		[Test]
		public void FlipXY()
		{
			ImageSpatialTransform transform = CreateTransform();
			transform.FlipX = true;
			transform.FlipY = true;

			Assert.AreEqual(-1.0f, transform.Transform.Elements[0]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[1]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[2]);
			Assert.AreEqual(-1.0f, transform.Transform.Elements[3]);
			Assert.AreEqual(transform.Scale, 1.0f);
			Assert.AreEqual(transform.ScaleX, 1.0f);
			Assert.AreEqual(transform.ScaleY, 1.0f);
		}

		[Test]
		public void Rotate1()
		{
			ImageSpatialTransform transform = CreateTransform();
			transform.ScaleToFit = false;
			transform.RotationXY = 90;

			Assert.IsTrue(Math.Abs(0.0f - transform.Transform.Elements[0]) < 1.0E-05);
			Assert.IsTrue(Math.Abs(1.0f - transform.Transform.Elements[1]) < 1.0E-05);
			Assert.IsTrue(Math.Abs(-1.0f - transform.Transform.Elements[2]) < 1.0E-05);
			Assert.IsTrue(Math.Abs(0.0f - transform.Transform.Elements[3]) < 1.0E-05);
		}

		[Test]
		public void Rotate2()
		{
			ImageSpatialTransform transform = CreateTransform();
			transform.ScaleToFit = false;

			transform.RotationXY = 0;
			Assert.AreEqual(0, transform.RotationXY);
			transform.RotationXY = 90;
			Assert.AreEqual(90, transform.RotationXY);
			transform.RotationXY = 360;
			Assert.AreEqual(0, transform.RotationXY);
			transform.RotationXY = 450;
			Assert.AreEqual(90, transform.RotationXY);
			transform.RotationXY = -90;
			Assert.AreEqual(270, transform.RotationXY);
			transform.RotationXY = -270;
			Assert.AreEqual(90, transform.RotationXY);
			transform.RotationXY = -450;
			Assert.AreEqual(270, transform.RotationXY);
		}

		[Test]
		public void Translate()
		{
			ImageSpatialTransform transform = CreateTransform();
			transform.TranslationX = 10;
			transform.TranslationY = 20;

			Assert.AreEqual(1.0f, transform.Transform.Elements[0]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[1]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[2]);
			Assert.AreEqual(1.0f, transform.Transform.Elements[3]);
			Assert.AreEqual(10.0f, transform.Transform.Elements[4]);
			Assert.AreEqual(20.0f, transform.Transform.Elements[5]);
		}

		[Test]
		public void ScaleToFit()
		{
			ImageSpatialTransform transform = CreateTransform();

			transform.ScaleToFit = false;
			transform.Scale = 2.0f;

			Assert.AreEqual(2.0f, transform.Transform.Elements[0]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[1]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[2]);
			Assert.AreEqual(2.0f, transform.Transform.Elements[3]);
			Assert.AreEqual(2.0f, transform.Scale);
			Assert.AreEqual(2.0f, transform.ScaleX);
			Assert.AreEqual(2.0f, transform.ScaleY);

			transform.ScaleToFit = true;

			Assert.AreEqual(1.0f, transform.Transform.Elements[0]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[1]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[2]);
			Assert.AreEqual(1.0f, transform.Transform.Elements[3]);
			Assert.AreEqual(1.0f, transform.Scale);
			Assert.AreEqual(1.0f, transform.ScaleX);
			Assert.AreEqual(1.0f, transform.ScaleY);
		}

		[Test]
		public void SourceToDestination()
		{
			// be sure to covert back and forth
			CompositeImageGraphic graphic = new CompositeImageGraphic(3062, 3732);
			ImageSpatialTransform transform = (ImageSpatialTransform)graphic.SpatialTransform;
			transform.ClientRectangle = new Rectangle(6, 6, 493, 626);

			PointF srcPt1 = new Point(100, 200);
			PointF dstPt = transform.ConvertToDestination(srcPt1);

			PointF srcPt2 = transform.ConvertToSource(dstPt);
			Assert.IsTrue(FloatComparer.AreEqual(srcPt1, srcPt2));
		}

		[Test]
		public void CumulativeTransform()
		{
			CompositeGraphic sceneGraph = new CompositeGraphic();

			CompositeGraphic graphic1 = new CompositeGraphic();
			graphic1.SpatialTransform.Scale = 2.0f;
			Assert.AreEqual(graphic1.SpatialTransform.CumulativeScale, 2.0f);
			Assert.AreEqual(graphic1.SpatialTransform.CumulativeTransform.Elements[0], 2.0f);

			CompositeGraphic graphic2 = new CompositeGraphic();
			graphic2.SpatialTransform.Scale = 3.0f;
			Assert.AreEqual(graphic2.SpatialTransform.CumulativeScale, 3.0f);
			Assert.AreEqual(graphic2.SpatialTransform.CumulativeTransform.Elements[0], 3.0f);
			
			CompositeGraphic graphic3 = new CompositeGraphic();
			graphic3.SpatialTransform.Scale = 4.0f;
			Assert.AreEqual(graphic3.SpatialTransform.CumulativeScale, 4.0f);
			Assert.AreEqual(graphic3.SpatialTransform.CumulativeTransform.Elements[0], 4.0f);

			graphic1.Graphics.Add(graphic2);
			Assert.AreEqual(graphic1.SpatialTransform.CumulativeScale, 2.0f);
			Assert.AreEqual(graphic1.SpatialTransform.CumulativeTransform.Elements[0], 2.0f);
			Assert.AreEqual(graphic2.SpatialTransform.CumulativeScale, 6.0f);
			Assert.AreEqual(graphic2.SpatialTransform.CumulativeTransform.Elements[0], 6.0f);

			graphic2.Graphics.Add(graphic3);
			Assert.AreEqual(graphic2.SpatialTransform.CumulativeScale, 6.0f);
			Assert.AreEqual(graphic2.SpatialTransform.CumulativeTransform.Elements[0], 6.0f);
			Assert.AreEqual(graphic3.SpatialTransform.CumulativeScale, 24.0f);
			Assert.AreEqual(graphic3.SpatialTransform.CumulativeTransform.Elements[0], 24.0f);

			sceneGraph.Graphics.Add(graphic1);
			Assert.AreEqual(graphic1.SpatialTransform.CumulativeScale, 2.0f);
			Assert.AreEqual(graphic1.SpatialTransform.CumulativeTransform.Elements[0], 2.0f);
			Assert.AreEqual(graphic2.SpatialTransform.CumulativeScale, 6.0f);
			Assert.AreEqual(graphic2.SpatialTransform.CumulativeTransform.Elements[0], 6.0f);
			Assert.AreEqual(graphic3.SpatialTransform.CumulativeScale, 24.0f);
			Assert.AreEqual(graphic3.SpatialTransform.CumulativeTransform.Elements[0], 24.0f);
		}

		[ExpectedException(typeof(ArgumentException))]
		[Test]
		public void TestRotationConstraints()
		{
			CompositeGraphic sceneGraph = CreateTestSceneGraph();
			//set the image graphic rotation to 90.
			sceneGraph.Graphics[0].SpatialTransform.RotationXY = 90;

			CompositeImageGraphic imageGraphic = (CompositeImageGraphic)sceneGraph.Graphics[0];
			CompositeGraphic primitiveOwner = (CompositeGraphic)imageGraphic.Graphics[0];
			Graphic primitive = (Graphic)primitiveOwner.Graphics[0];

			sceneGraph.SpatialTransform.RotationXY = 90;
			imageGraphic.SpatialTransform.RotationXY = 90;
			primitiveOwner.SpatialTransform.RotationXY = 10;
			primitive.SpatialTransform.RotationXY = 20;

			Assert.AreEqual(sceneGraph.SpatialTransform.CumulativeRotationXY, 90); 
			Assert.AreEqual(imageGraphic.SpatialTransform.CumulativeRotationXY, 180);
			Assert.AreEqual(primitiveOwner.SpatialTransform.CumulativeRotationXY, 190);
			Assert.AreEqual(primitive.SpatialTransform.CumulativeRotationXY, 210);

			try
			{
				//this will throw an exception.
				sceneGraph.SpatialTransform.RotationXY = 30;
			}
			catch
			{
				//rotation remains at 90.
				Assert.AreEqual(sceneGraph.SpatialTransform.CumulativeRotationXY, 90);
				throw;
			}
		}

		private static CompositeGraphic CreateTestSceneGraph()
		{
			CompositeGraphic sceneGraph = new CompositeGraphic();
			ImageSpatialTransform imageTransform = CreateTransform();

			sceneGraph.Graphics.Add(imageTransform.OwnerGraphic);

			CompositeGraphic composite = new CompositeGraphic();
			Graphic leaf = new LinePrimitive();
			composite.Graphics.Add(leaf);

			((CompositeImageGraphic)imageTransform.OwnerGraphic).Graphics.Add(composite);

			return sceneGraph;
		}

		private static ImageSpatialTransform CreateTransform()
		{
			CompositeImageGraphic graphic = new CompositeImageGraphic(512, 384);
			ImageSpatialTransform transform = (ImageSpatialTransform)graphic.SpatialTransform;
			transform.ClientRectangle = new Rectangle(0, 0, 384, 512);
			return transform;
		}

		//[Test]
		//public void QuadrantTransformSameImageSize()
		//{
		//    SpatialTransform transform = new SpatialTransform();
		//    transform.SourceRectangle = new Rectangle(0, 0, 7, 11);
		//    transform.DestinationRectangle = new Rectangle(0, 0, 7, 11);

		//    int srcWidth = transform.SourceRectangle.Width;
		//    int srcHeight = transform.SourceRectangle.Height;
		//    int dstWidth = transform.DestinationRectangle.Width;
		//    int dstHeight = transform.DestinationRectangle.Height;
		//    float oneQuarter = 0.25f;
		//    float threeQuarters = 0.75f;

		//    PointF destinationPoint = new PointF((dstWidth * oneQuarter), (dstHeight * oneQuarter));
		//    PointF sourcePointExpected = new PointF((srcWidth * oneQuarter), (srcHeight * oneQuarter));
		//    PointF SourcePointCalculated = transform.ConvertToSource(destinationPoint);
		//    PointF destinationPointBackCalculated = transform.ConvertToDestination(SourcePointCalculated);
		//    Assert.IsTrue(sourcePointExpected == SourcePointCalculated);
		//    Assert.IsTrue(destinationPointBackCalculated == destinationPoint);

		//    transform.FlipHorizontal = true;

		//    sourcePointExpected.X = (srcWidth * threeQuarters);
		//    SourcePointCalculated = transform.ConvertToSource(destinationPoint);
		//    destinationPointBackCalculated = transform.ConvertToDestination(SourcePointCalculated);
		//    Assert.IsTrue(sourcePointExpected == SourcePointCalculated);
		//    Assert.IsTrue(destinationPointBackCalculated == destinationPoint);

		//    transform.FlipHorizontal = false;
		//    transform.FlipVertical = true;

		//    sourcePointExpected.X = (srcWidth * oneQuarter);
		//    sourcePointExpected.Y = (srcHeight * threeQuarters);
		//    SourcePointCalculated = transform.ConvertToSource(destinationPoint);
		//    destinationPointBackCalculated = transform.ConvertToDestination(SourcePointCalculated);
		//    Assert.IsTrue(sourcePointExpected == SourcePointCalculated);
		//    Assert.IsTrue(destinationPointBackCalculated == destinationPoint);

		//    transform.FlipHorizontal = true;
		//    transform.FlipVertical = true;

		//    sourcePointExpected.X = (srcWidth * threeQuarters);
		//    sourcePointExpected.Y = (srcHeight * threeQuarters);
		//    SourcePointCalculated = transform.ConvertToSource(destinationPoint);
		//    destinationPointBackCalculated = transform.ConvertToDestination(SourcePointCalculated);
		//    Assert.IsTrue(sourcePointExpected == SourcePointCalculated);
		//    Assert.IsTrue(destinationPointBackCalculated == destinationPoint);
		//}

		//[Test]
		//public void TransformVectorsTest()
		//{
		//    PointF[] originalVectors = new PointF[] { new PointF(1, 1), new PointF(1, -1), new PointF(-1, 1), new PointF(-1, -1) };

		//    SpatialTransform transform = new SpatialTransform();
		//    transform.SourceRectangle = new Rectangle(0, 0, 10, 10);
		//    transform.DestinationRectangle = new Rectangle(0, 0, 15, 25);

		//    PointF[] testVectors;
		//    PointF[] resultVectors;

		//    testVectors = (PointF[])originalVectors.Clone();
		//    resultVectors = new PointF[] { new PointF(1.5F, 1.5F), new PointF(1.5F, -1.5F), new PointF(-1.5F, 1.5F), new PointF(-1.5F, -1.5F) };
		//    transform.ConvertVectorsToDestination(testVectors);
		//    Assert.IsTrue(VerifyTestVectors(testVectors, resultVectors));

		//    resultVectors = (PointF[])originalVectors.Clone();
		//    transform.ConvertVectorsToSource(testVectors);
		//    Assert.IsTrue(VerifyTestVectors(testVectors, resultVectors));

		//    testVectors = (PointF[])originalVectors.Clone();
		//    resultVectors = new PointF[] { new PointF(-1.5F, 1.5F), new PointF(-1.5F, -1.5F), new PointF(1.5F, 1.5F), new PointF(1.5F, -1.5F) };
		//    transform.FlipHorizontal = true;
		//    transform.ConvertVectorsToDestination(testVectors);
		//    Assert.IsTrue(VerifyTestVectors(testVectors, resultVectors));

		//    resultVectors = (PointF[])originalVectors.Clone();
		//    transform.ConvertVectorsToSource(testVectors);
		//    Assert.IsTrue(VerifyTestVectors(testVectors, resultVectors));

		//    testVectors = (PointF[])originalVectors.Clone();
		//    resultVectors = new PointF[] { new PointF(-1.5F, -1.5F), new PointF(-1.5F, 1.5F), new PointF(1.5F, -1.5F), new PointF(1.5F, 1.5F) };
		//    transform.FlipVertical = true;
		//    transform.ConvertVectorsToDestination(testVectors);
		//    Assert.IsTrue(VerifyTestVectors(testVectors, resultVectors));

		//    resultVectors = (PointF[])originalVectors.Clone();
		//    transform.ConvertVectorsToSource(testVectors);
		//    Assert.IsTrue(VerifyTestVectors(testVectors, resultVectors));

		//    testVectors = (PointF[])originalVectors.Clone();
		//    resultVectors = new PointF[] { new PointF(-3F, -3F), new PointF(-3F, 3F), new PointF(3F, -3F), new PointF(3F, 3F) };
		//    transform.ScaleToFit = false;
		//    transform.Scale = 3F;
		//    transform.ConvertVectorsToDestination(testVectors);
		//    Assert.IsTrue(VerifyTestVectors(testVectors, resultVectors));

		//    resultVectors = (PointF[])originalVectors.Clone();
		//    transform.ConvertVectorsToSource(testVectors);
		//    Assert.IsTrue(VerifyTestVectors(testVectors, resultVectors));

		//    testVectors = (PointF[])originalVectors.Clone();
		//    resultVectors = new PointF[] { new PointF(-3F, 3F), new PointF(3F, 3F), new PointF(-3F, -3F), new PointF(3F, -3F) };
		//    transform.Rotation = -90;
		//    transform.ConvertVectorsToDestination(testVectors);
		//    Assert.IsTrue(VerifyTestVectors(testVectors, resultVectors));

		//    resultVectors = (PointF[])originalVectors.Clone();
		//    transform.ConvertVectorsToSource(testVectors);
		//    Assert.IsTrue(VerifyTestVectors(testVectors, resultVectors));
		//}

		//private bool VerifyTestVectors(PointF[] transformed, PointF[] expected)
		//{
		//    for (int i = 0; i < transformed.Length; ++i)
		//    {
		//        double xDifference = Math.Abs(transformed[i].X - expected[i].X);
		//        double yDifference = Math.Abs(transformed[i].Y - expected[i].Y);

		//        if (xDifference > 0.001 || yDifference > 0.001)
		//            return false;
		//    }

		//    return true;
		//}

	}
}

#endif