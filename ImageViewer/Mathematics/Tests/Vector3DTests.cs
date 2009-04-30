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

using NUnit.Framework;
using System;

namespace ClearCanvas.ImageViewer.Mathematics.Tests
{
	[TestFixture]
	public class Vector3DTests
	{
		public Vector3DTests()
		{
		}

		[Test]
		public void TestAdd()
		{
			Vector3D v1 = new Vector3D(2.2F, 6.1F, 7.4F);
			Vector3D v2 = new Vector3D(3.8F, 3.7F, 4.1F);
			Vector3D result = new Vector3D(6F, 9.8F, 11.5F);

			Assert.IsTrue(Vector3D.AreEqual(v1 + v2, result));

			v1 = new Vector3D(2.2F, -6.1F, 7.4F);
			v2 = new Vector3D(-3.8F, 3.7F, -4.1F);
			result = new Vector3D(-1.6F, -2.4F, 3.3F);

			Assert.IsTrue(Vector3D.AreEqual(v1 + v2, result));
		}

		[Test]
		public void TestSubtract()
		{
			Vector3D v1 = new Vector3D(2.2F, 6.1F, 7.4F);
			Vector3D v2 = new Vector3D(3.8F, 3.7F, 4.1F);
			Vector3D result = new Vector3D(-1.6F, 2.4F, 3.3F);

			Assert.IsTrue(Vector3D.AreEqual(v1 - v2, result));

			v1 = new Vector3D(2.2F, -6.1F, 7.4F);
			v2 = new Vector3D(-3.8F, 3.7F, -4.1F);
			result = new Vector3D(6F, -9.8F, 11.5F);

			Assert.IsTrue(Vector3D.AreEqual(v1 - v2, result));
		}

		[Test]
		public void TestMultiply()
		{
			Vector3D v1 = new Vector3D(2.2F, -6.1F, 7.4F);
			Vector3D result = new Vector3D(6.82F, -18.91F, 22.94f);
			v1 = 3.1F*v1;

			Assert.IsTrue(Vector3D.AreEqual(v1, result));
		}

		[Test]
		public void TestDivide()
		{
			Vector3D result = new Vector3D(2.2F, -6.1F, 7.4F);
			Vector3D v1 = new Vector3D(6.82F, -18.91F, 22.94f);
			v1 = 3.1F / v1;

			Assert.IsTrue(Vector3D.AreEqual(v1, result));
		}

		[Test]
		public void TestNormalize()
		{
			Vector3D v1 = new Vector3D(2.2F, -6.1F, 7.4F);
			Assert.IsTrue(FloatComparer.AreEqual(v1.Magnitude, 9.8392072851F));

			Vector3D normalized = v1.Normalize();
			Assert.IsTrue(FloatComparer.AreEqual(normalized.Magnitude, 1.0F));
		}

		[Test]
		public void TestDot()
		{
			Vector3D v1 = new Vector3D(2.2F, -6.1F, 7.4F);
			Vector3D v2 = new Vector3D(3.8F, 3.7F, 4.1F);
			
			Assert.IsTrue(FloatComparer.AreEqual(v1.Dot(v2), 16.13F));
		}

		[Test]
		public void TestCross()
		{
			Vector3D v1 = new Vector3D(2.2F, -6.1F, 7.4F);
			Vector3D v2 = new Vector3D(-3.8F, 3.7F, 4.1F);
			Vector3D result = new Vector3D(-52.39F, -37.14F, -15.04F);

			Assert.IsTrue(Vector3D.AreEqual(v1.Cross(v2), result));
		}
		
		[Test]
		public void TestLinePlaneIntersection()
		{
			Vector3D planeNormal = new Vector3D(1, 1, 1);
			Vector3D pointInPlane = new Vector3D(1, 1, 1);

			Vector3D point1 = new Vector3D(0.5F, 0.5F, 0.5F);
			Vector3D point2 = new Vector3D(1.5F, 1.5F, 1.5F);

			Vector3D expected = new Vector3D(1, 1, 1);
			Vector3D intersection = Vector3D.GetLinePlaneIntersection(planeNormal, pointInPlane, point1, point2, true);

			// line segment intersects plane
			Assert.IsTrue(Vector3D.AreEqual(expected, intersection), "line-plane intersection is not what is expected!");

			// infinite line intersects plane
			point2 = -point2;
			intersection = Vector3D.GetLinePlaneIntersection(planeNormal, pointInPlane, point1, point2, false);
			Assert.IsTrue(Vector3D.AreEqual(expected, intersection), "line-plane intersection is not what is expected!");

			// line segment does not intersect plane
			intersection = Vector3D.GetLinePlaneIntersection(planeNormal, pointInPlane, point1, point2, true);
			Assert.AreEqual(intersection, null, "line-plane intersection is not what is expected!");

			// line is in plane (no intersection)
			point1 = new Vector3D(1, 0, 0);
			point2 = new Vector3D(0, 0, 1);
			intersection = Vector3D.GetLinePlaneIntersection(planeNormal, pointInPlane, point1, point2, true);
			Assert.AreEqual(intersection, null, "line-plane intersection is not what is expected!");

			// line is in plane (no intersection)
			intersection = Vector3D.GetLinePlaneIntersection(planeNormal, pointInPlane, point1, point2, false);
			Assert.AreEqual(intersection, null, "line-plane intersection is not what is expected!");

			// intersection at infinity (sort of)
			point1 = new Vector3D(1, 0, 0);
			point2 = new Vector3D(0, 0, 0.99999991F);
			intersection = Vector3D.GetLinePlaneIntersection(planeNormal, pointInPlane, point1, point2, true);
			Assert.AreEqual(intersection, null, "line-plane intersection is not what is expected!");

			// intersection at infinity (sort of), line segment does not intersect
			intersection = Vector3D.GetLinePlaneIntersection(planeNormal, pointInPlane, point1, point2, false);
			Assert.AreNotEqual(intersection, null, "line-plane intersection is not what is expected!");
		}
	}
}

#endif