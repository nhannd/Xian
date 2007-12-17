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
