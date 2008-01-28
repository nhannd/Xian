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

using System;
using System.Drawing;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.InteractiveGraphics.Tests
{
	[TestFixture]
	public class EllipseInteractiveGraphicTest
	{
		public EllipseInteractiveGraphicTest()
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
		public void EllipseIntersectionTest()
		{
			float inverseRoot2 = (float) (1/Math.Sqrt(2));

			float a = 1;
			float b = 1;
			PointF center = new PointF(2, 2);
			PointF point = center + new SizeF(1,1);
			PointF intersectPoint = EllipseInteractiveGraphic.IntersectEllipseAndLine(a, b, center, point);
			Assert.AreEqual(center.X + inverseRoot2, intersectPoint.X);
			Assert.AreEqual(center.Y + inverseRoot2, intersectPoint.Y);

			point = center + new SizeF(-1, 1);
			intersectPoint = EllipseInteractiveGraphic.IntersectEllipseAndLine(a, b, center, point);
			Assert.AreEqual(center.X - inverseRoot2, intersectPoint.X);
			Assert.AreEqual(center.Y + inverseRoot2, intersectPoint.Y);

			point = center + new SizeF(-1, -1);
			intersectPoint = EllipseInteractiveGraphic.IntersectEllipseAndLine(a, b, center, point);
			Assert.AreEqual(center.X - inverseRoot2, intersectPoint.X);
			Assert.AreEqual(center.Y - inverseRoot2, intersectPoint.Y);

			point = center + new SizeF(1, -1);
			intersectPoint = EllipseInteractiveGraphic.IntersectEllipseAndLine(a, b, center, point);
			Assert.AreEqual(center.X + inverseRoot2, intersectPoint.X);
			Assert.AreEqual(center.Y - inverseRoot2, intersectPoint.Y);

			a = 2;
			b = 1;
			point = center + new SizeF(0, 2);
			intersectPoint = EllipseInteractiveGraphic.IntersectEllipseAndLine(a, b, center, point);
			Assert.AreEqual(center.X, intersectPoint.X);
			Assert.AreEqual(center.Y + 1, intersectPoint.Y);

			point = center + new SizeF(3, 0);
			intersectPoint = EllipseInteractiveGraphic.IntersectEllipseAndLine(a, b, center, point);
			Assert.AreEqual(center.X + 2, intersectPoint.X);
			Assert.AreEqual(center.Y, intersectPoint.Y);

		}
	}
}

#endif