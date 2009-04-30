#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

using System;
using System.Drawing;
using ClearCanvas.ImageViewer.Graphics;
using NUnit.Framework;
using ClearCanvas.ImageViewer.Mathematics;
namespace ClearCanvas.ImageViewer.InteractiveGraphics.Tests
{
	[TestFixture]
	public class EllipseIntersectionTests
	{
		public EllipseIntersectionTests()
		{
		}

		private static void VerifyPointOnEllipse(float a, float b, PointF center, PointF intersection)
		{
			float x = intersection.X - center.X;
			float y = intersection.Y - center.Y;

			float rule = x * x / (a * a) + y * y / (b * b);
			Assert.IsTrue(FloatComparer.AreEqual(1F, rule), "Point is not on ellipse!");
		}
		
		[Test]
		public void SimpleTest()
		{
			//for a circle of radius=1, test point = 1,1, intesection is 1/sqrt(2), 1/sqrt(2)
			float a = 1F;
			float b = 1F;
			PointF center = new PointF(0, 0);
			float root2Inverse = 1F/(float)Math.Sqrt(2);
			PointF result = new PointF(root2Inverse, root2Inverse);
			PointF intersection = EllipsePrimitive.IntersectEllipseAndLine(a, b, center, new PointF(1, 1));

			VerifyPointOnEllipse(a, b, center, intersection);

			Assert.IsTrue(FloatComparer.AreEqual(result, intersection), "ellipse intersection point is not correct!");
		}

		[Test]
		public void TestPointInside()
		{
			float a = 2.5F;
			float b = 1.25F;
			PointF center = new PointF(-3F, -2F);
			PointF test = new PointF(-4F, -2.5F);
			PointF result = new PointF(-4.767767F, -2.883884F);

			PointF intersection = EllipsePrimitive.IntersectEllipseAndLine(a, b, center, test);

			VerifyPointOnEllipse(a, b, center, intersection);

			Assert.IsTrue(FloatComparer.AreEqual(result, intersection), "ellipse intersection point is not correct!");
		}

		[Test]
		public void TestPointOutside()
		{
			float a = -2.5F;
			float b = -1.25F;
			PointF center = new PointF(3F, 2F);
			PointF test = new PointF(13F, 7F);
			PointF result = new PointF(4.767767F, 2.883884F);

			PointF intersection = EllipsePrimitive.IntersectEllipseAndLine(a, b, center, test);

			VerifyPointOnEllipse(a, b, center, intersection);

			Assert.IsTrue(FloatComparer.AreEqual(result, intersection), "ellipse intersection point is not correct!");
		}


		[Test]
		public void TestPointNearZero()
		{
			float a = 2.5F;
			float b = 1.25F;
			PointF center = new PointF(3F, 2F);
			PointF test = new PointF(3.01F, 2.01F);

			PointF intersection = EllipsePrimitive.IntersectEllipseAndLine(a, b, center, test);
			VerifyPointOnEllipse(a, b, center, intersection);
		}

		[Test]
		public void TestPointVeryCloseToZero()
		{
			float a = 2.5F;
			float b = 1.25F;
			PointF center = new PointF(3F, 2F);
			PointF test = new PointF(3.0001F, 2.0001F);

			PointF intersection = EllipsePrimitive.IntersectEllipseAndLine(a, b, center, test);
			
			//intersection is at center.
			Assert.IsTrue(FloatComparer.AreEqual(center, intersection), "ellipse intersection point is not correct!");
		}

		[Test]
		public void TestMajorAxisZero()
		{
			float a = 0F;
			float b = 1.25F;
			PointF center = new PointF(3F, 2F);
			PointF test = new PointF(13F, 7F);

			PointF intersection = EllipsePrimitive.IntersectEllipseAndLine(a, b, center, test);

			//intersection is at center.
			Assert.IsTrue(FloatComparer.AreEqual(center, intersection), "ellipse intersection point is not correct!");
		}

		[Test]
		public void TestMinorAxisZero()
		{
			float a = 2.5F;
			float b = 0F;
			PointF center = new PointF(3F, 2F);
			PointF test = new PointF(13F, 7F);

			PointF intersection = EllipsePrimitive.IntersectEllipseAndLine(a, b, center, test);

			//intersection is at center.
			Assert.IsTrue(FloatComparer.AreEqual(center, intersection), "ellipse intersection point is not correct!");
		}
	}
}

#endif