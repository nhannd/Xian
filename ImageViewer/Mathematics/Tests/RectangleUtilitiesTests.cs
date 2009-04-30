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

using System.Drawing;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Mathematics.Tests
{

	[TestFixture]
	public class RectangleUtilitiesTests
	{
		public RectangleUtilitiesTests()
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
		public void Containment()
		{
			RectangleF rect1 = new RectangleF(0, 0, 10, 10);
			RectangleF rect2 = new RectangleF(2, 2, 5, 5);

			RectangleF intersectRect = RectangleUtilities.Intersect(rect1, rect2);

			Assert.AreEqual(rect2, intersectRect);
		}

		[Test]
		public void IntersectOn1Side()
		{
			RectangleF rect1 = new RectangleF(2, -2, 5, 5); 
			RectangleF rect2 = new RectangleF(0, 0, 10, 10);

			RectangleF intersectRect = RectangleUtilities.Intersect(rect1, rect2);

			Assert.AreEqual(new RectangleF(2,0,5,3), intersectRect);

			rect1 = new RectangleF(0, 0, -10, 10); 
			rect2 = new RectangleF(-8, -2, 5, 5);

			intersectRect = RectangleUtilities.Intersect(rect1, rect2);

			Assert.AreEqual(new RectangleF(-3, 0, -5, 3), intersectRect);

			rect1 = new RectangleF(0, 0, -10, 10);
			rect2 = new RectangleF(-3, -2, -5, 5);

			intersectRect = RectangleUtilities.Intersect(rect1, rect2);

			Assert.AreEqual(new RectangleF(-3, 0, -5, 3), intersectRect);
		}

		[Test]
		public void IntersectOn2Sides()
		{
			RectangleF rect1 = new RectangleF(0, 0, 10, 10);
			RectangleF rect2 = new RectangleF(-2, -2, 5, 5);

			RectangleF intersectRect = RectangleUtilities.Intersect(rect1, rect2);

			Assert.AreEqual(new RectangleF(0, 0, 3, 3), intersectRect);
		}
		
		[Test]
		public void NoIntersection()
		{
			RectangleF rect1 = new RectangleF(0, 0, 10, 10);
			RectangleF rect2 = new RectangleF(-20, 0, 10, 10);

			RectangleF intersectRect = RectangleUtilities.Intersect(rect1, rect2);

			Assert.IsTrue(intersectRect.IsEmpty);
		}

		[Test]
		public void NoIntersection1()
		{
			RectangleF rect1 = new RectangleF(0, 0, 10, 10);
			RectangleF rect2 = new RectangleF(11, 0, 10, 10);

			Assert.IsFalse(RectangleUtilities.DoesIntersect(rect1, rect2));

			rect2 = new RectangleF(0, 0, 10, 10);
			rect1 = new RectangleF(11, 0, 10, 10);

			Assert.IsFalse(RectangleUtilities.DoesIntersect(rect1, rect2));
		}

		[Test]
		public void NoIntersection2()
		{
			RectangleF rect1 = new RectangleF(0, 0, 10, 10);
			RectangleF rect2 = new RectangleF(21, 0, -10, 10);

			Assert.IsFalse(RectangleUtilities.DoesIntersect(rect1, rect2));

			rect2 = new RectangleF(0, 0, 10, 10);
			rect1 = new RectangleF(21, 0, -10, 10);

			Assert.IsFalse(RectangleUtilities.DoesIntersect(rect1, rect2));
		}

		[Test]
		public void NoIntersection3()
		{
			RectangleF rect1 = new RectangleF(0, 0, -10, 10);
			RectangleF rect2 = new RectangleF(1, 0, 10, 10);

			Assert.IsFalse(RectangleUtilities.DoesIntersect(rect1, rect2));

			rect2 = new RectangleF(0, 0, -10, 10);
			rect1 = new RectangleF(1, 0, 10, 10);

			Assert.IsFalse(RectangleUtilities.DoesIntersect(rect1, rect2));
		}

		[Test]
		public void NoIntersection4()
		{
			RectangleF rect1 = new RectangleF(0, 0, -10, 10);
			RectangleF rect2 = new RectangleF(11, 0, -10, 10);

			Assert.IsFalse(RectangleUtilities.DoesIntersect(rect1, rect2));

			rect2 = new RectangleF(0, 0, -10, 10);
			rect1 = new RectangleF(11, 0, -10, 10);

			Assert.IsFalse(RectangleUtilities.DoesIntersect(rect1, rect2));
		}

		[Test]
		public void Touching()
		{
			RectangleF rect1 = new RectangleF(0, 0, 10, 10);
			RectangleF rect2 = new RectangleF(10, 0, 10, 10);

			Assert.IsFalse(RectangleUtilities.DoesIntersect(rect1, rect2));

			rect2 = new RectangleF(0, 0, 10, 10);
			rect1 = new RectangleF(10, 0, 10, 10);

			Assert.IsFalse(RectangleUtilities.DoesIntersect(rect1, rect2));
		}

		[Test]
		public void RoundInflated()
		{
			//positive width and height
			RectangleF testRectangle = new RectangleF(1.5F, 2.5F, 5F, 7F);
			testRectangle = RectangleUtilities.RoundInflate(testRectangle);
			Assert.AreEqual(testRectangle.Left, 1);
			Assert.AreEqual(testRectangle.Top, 2);
			Assert.AreEqual(testRectangle.Right, 7);
			Assert.AreEqual(testRectangle.Bottom, 10);

			testRectangle = new RectangleF(-1.5F, -2.5F, 5F, 7F);
			testRectangle = RectangleUtilities.RoundInflate(testRectangle);
			Assert.AreEqual(testRectangle.Left, -2);
			Assert.AreEqual(testRectangle.Top, -3);
			Assert.AreEqual(testRectangle.Right, 4);
			Assert.AreEqual(testRectangle.Bottom, 5);

			testRectangle = new RectangleF(-11.5F, -12.5F, 5F, 7F);
			testRectangle = RectangleUtilities.RoundInflate(testRectangle);
			Assert.AreEqual(testRectangle.Left, -12);
			Assert.AreEqual(testRectangle.Top, -13);
			Assert.AreEqual(testRectangle.Right, -6);
			Assert.AreEqual(testRectangle.Bottom, -5);

			//negative width and height
			testRectangle = new RectangleF(10.5F, 11.5F, -5F, -7F);
			testRectangle = RectangleUtilities.RoundInflate(testRectangle);
			Assert.AreEqual(testRectangle.Left, 11);
			Assert.AreEqual(testRectangle.Top, 12);
			Assert.AreEqual(testRectangle.Right, 5);
			Assert.AreEqual(testRectangle.Bottom, 4);

			testRectangle = new RectangleF(-10.5F, -11.5F, -5F, -7F);
			testRectangle = RectangleUtilities.RoundInflate(testRectangle);
			Assert.AreEqual(testRectangle.Left, -10);
			Assert.AreEqual(testRectangle.Top, -11);
			Assert.AreEqual(testRectangle.Right, -16);
			Assert.AreEqual(testRectangle.Bottom, -19);

			testRectangle = new RectangleF(3.5F, 5.5F, -5F, -7F);
			testRectangle = RectangleUtilities.RoundInflate(testRectangle);
			Assert.AreEqual(testRectangle.Left, 4);
			Assert.AreEqual(testRectangle.Top, 6);
			Assert.AreEqual(testRectangle.Right, -2);
			Assert.AreEqual(testRectangle.Bottom, -2);
		}

		[Test]
		public void ConvertToPositiveRectangleTest()
		{
			RectangleF rect = new RectangleF(1,2,5,6);
			RectangleF posRect = RectangleUtilities.ConvertToPositiveRectangle(rect);

			Assert.AreEqual(rect, posRect);

			rect = new RectangleF(1, 2, -5, 6);
			posRect = RectangleUtilities.ConvertToPositiveRectangle(rect);
			Assert.AreEqual(new RectangleF(-4,2,5,6), posRect);

			rect = new RectangleF(1, 2, 5, -6);
			posRect = RectangleUtilities.ConvertToPositiveRectangle(rect);
			Assert.AreEqual(new RectangleF(1,-4,5,6), posRect);

			rect = new RectangleF(1, 2, -5, -6);
			posRect = RectangleUtilities.ConvertToPositiveRectangle(rect);
			Assert.AreEqual(new RectangleF(-4, -4, 5, 6), posRect);
		}
	}
}
#endif