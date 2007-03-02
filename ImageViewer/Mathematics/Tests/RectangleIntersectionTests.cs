#if	UNIT_TESTS

#pragma warning disable 1591,0419,1574,1587

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using NUnit.Framework;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Rendering;

namespace ClearCanvas.ImageViewer.Mathematics.Tests
{

	[TestFixture]
	public class RectangleIntersectionTests
	{
		public RectangleIntersectionTests()
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
	}
}
#endif