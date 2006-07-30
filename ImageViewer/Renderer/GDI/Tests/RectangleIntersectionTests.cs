#if	UNIT_TESTS

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using NUnit.Framework;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Layers;
using ClearCanvas.ImageViewer.Renderer.GDI;

namespace ClearCanvas.ImageViewer.Imaging.Tests
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
			RectangleF rect1 = new RectangleF(0, 0, 10, 10);
			RectangleF rect2 = new RectangleF(2, -2, 5, 5);

			RectangleF intersectRect = RectangleUtilities.Intersect(rect1, rect2);

			Assert.AreEqual(new RectangleF(2,0,5,3), intersectRect);

			rect1 = new RectangleF(0, 0, -10, 10);
			rect2 = new RectangleF(-8, -2, 5, 5);

			intersectRect = RectangleUtilities.Intersect(rect1, rect2);

			Assert.AreEqual(new RectangleF(-8, 0, 5, 3), intersectRect);

			rect2 = new RectangleF(0, 0, -10, 10);
			rect1 = new RectangleF(-8, -2, 5, 5);

			intersectRect = RectangleUtilities.Intersect(rect1, rect2);

			Assert.AreEqual(new RectangleF(-8, 0, 5, 3), intersectRect);

			rect1 = new RectangleF(0, 0, -10, 10);
			rect2 = new RectangleF(-3, -2, -5, 5);

			intersectRect = RectangleUtilities.Intersect(rect1, rect2);

			Assert.AreEqual(new RectangleF(-8, 0, 5, 3), intersectRect);
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
	}
}
#endif