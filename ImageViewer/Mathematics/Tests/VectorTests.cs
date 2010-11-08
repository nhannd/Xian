#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if	UNIT_TESTS

#pragma warning disable 1591,0419,1574,1587

using System;
using System.Drawing;
using ClearCanvas.Common;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Mathematics.Tests
{
	[TestFixture]
	public class VectorTests
	{
		[Test]
		public void TestSubtendedAngle()
		{
			double angle = Vector.SubtendedAngle(new PointF(10, 0), new PointF(0, 0), new PointF(1, 0));
			Assert.AreEqual(0, angle);

			angle = Vector.SubtendedAngle(new PointF(10, 0), new PointF(0, 0), new PointF(0, 1));
			Assert.AreEqual(-90, angle);

			angle = Vector.SubtendedAngle(new PointF(0, 10), new PointF(0, 0), new PointF(1, 0));
			Assert.AreEqual(90, angle);

			angle = Vector.SubtendedAngle(new PointF(10, 0), new PointF(0, 0), new PointF(-1, 0));
			Assert.AreEqual(180, angle);
		}

		[Test]
		public void TestUnitVector()
		{
			float oneOverSqrt2 = 1/(float) Math.Sqrt(2);
			SizeF unitVector;
			PointF origin;

			origin = new PointF(0, 0);
			unitVector = new SizeF(Vector.CreateUnitVector(origin, origin + new SizeF(-1, 1)));
			Assert.IsTrue(FloatComparer.AreEqual(new SizeF(-oneOverSqrt2, oneOverSqrt2), unitVector));
			Assert.IsTrue(FloatComparer.AreEqual(unitVector.Width*unitVector.Width + unitVector.Height*unitVector.Height, 1), "Magnitude must be 1.");

			origin = new PointF(-1, 1);
			unitVector = new SizeF(Vector.CreateUnitVector(origin, origin + new SizeF(-2, 2)));
			Assert.IsTrue(FloatComparer.AreEqual(new SizeF(-oneOverSqrt2, oneOverSqrt2), unitVector));
			Assert.IsTrue(FloatComparer.AreEqual(unitVector.Width*unitVector.Width + unitVector.Height*unitVector.Height, 1), "Magnitude must be 1.");

			origin = new PointF(3, -1);
			unitVector = new SizeF(Vector.CreateUnitVector(origin, origin + new SizeF(-2, 2)));
			Assert.IsTrue(FloatComparer.AreEqual(new SizeF(-oneOverSqrt2, oneOverSqrt2), unitVector));
			Assert.IsTrue(FloatComparer.AreEqual(unitVector.Width*unitVector.Width + unitVector.Height*unitVector.Height, 1), "Magnitude must be 1.");

			origin = new PointF(3, 1);
			unitVector = new SizeF(Vector.CreateUnitVector(origin, origin + new SizeF(-2, 2)));
			Assert.IsTrue(FloatComparer.AreEqual(new SizeF(-oneOverSqrt2, oneOverSqrt2), unitVector));
			Assert.IsTrue(FloatComparer.AreEqual(unitVector.Width*unitVector.Width + unitVector.Height*unitVector.Height, 1), "Magnitude must be 1.");
		}

		[Test]
		public void TestDistance()
		{
			var root2 = Math.Sqrt(2);
			var p0 = new PointF(0, 0);
			var p1 = new PointF(-1, 1);
			var p2 = new PointF(1, -1);
			var p3 = new PointF(-1, -1);
			var p4 = new PointF(-2, -2);

			// exercise zero computations
			Assert.AreEqual(0, Vector.Distance(p0, p0), "P0 to P0 distance should be 0");
			Assert.AreEqual(0, Vector.Distance(p1, p1), "P1 to P1 distance should be 0");
			Assert.AreEqual(0, Vector.Distance(p2, p2), "P2 to P2 distance should be 0");
			Assert.AreEqual(0, Vector.Distance(p3, p3), "P3 to P3 distance should be 0");

			// exercise computation with origin
			Assert.AreEqual(root2, Vector.Distance(p0, p1), "P0 to P1 distance should be \u221A2");
			Assert.AreEqual(root2, Vector.Distance(p1, p0), "P1 to P0 distance should be \u221A2");
			Assert.AreEqual(root2, Vector.Distance(p0, p2), "P0 to P2 distance should be \u221A2");
			Assert.AreEqual(root2, Vector.Distance(p2, p0), "P2 to P0 distance should be \u221A2");
			Assert.AreEqual(root2, Vector.Distance(p0, p3), "P0 to P3 distance should be \u221A2");
			Assert.AreEqual(root2, Vector.Distance(p3, p0), "P3 to P0 distance should be \u221A2");

			// exercise cross-quadrant computations
			Assert.AreEqual(2*root2, Vector.Distance(p1, p2), "P1 to P2 distance should be 2\u221A2");
			Assert.AreEqual(2*root2, Vector.Distance(p2, p1), "P2 to P1 distance should be 2\u221A2");
			Assert.AreEqual(2, Vector.Distance(p1, p3), "P1 to P3 distance should be 2");
			Assert.AreEqual(2, Vector.Distance(p3, p1), "P3 to P1 distance should be 2");
			Assert.AreEqual(2, Vector.Distance(p2, p3), "P2 to P3 distance should be 2");
			Assert.AreEqual(2, Vector.Distance(p3, p2), "P3 to P2 distance should be 2");

			// exercise same-quadrant computation
			Assert.AreEqual(root2, Vector.Distance(p3, p4), "P3 to P4 distance should be \u221A2");
			Assert.AreEqual(root2, Vector.Distance(p4, p3), "P4 to P3 distance should be \u221A2");
		}

		[Test]
		public void TestMidpoint()
		{
			var root2F = (float) Math.Sqrt(2);
			var p0 = new PointF(0, 0);
			var p1 = new PointF(-1, 1);
			var p2 = new PointF(1, -1);
			var p3 = new PointF(-1, -1);
			var p4 = new PointF(-2, -2);

			// exercise zero computations
			Assert.AreEqual(p0, Vector.Midpoint(p0, p0), "P0 to P0 midpoint should be P0");
			Assert.AreEqual(p1, Vector.Midpoint(p1, p1), "P1 to P1 midpoint should be P1");
			Assert.AreEqual(p2, Vector.Midpoint(p2, p2), "P2 to P2 midpoint should be P2");
			Assert.AreEqual(p3, Vector.Midpoint(p3, p3), "P3 to P3 midpoint should be P3");

			// exercise computation with origin
			Assert.AreEqual(new PointF(-1/2f, 1/2f), Vector.Midpoint(p0, p1), "P0 to P1 midpoint should be (-\u00BD,\u00BD)");
			Assert.AreEqual(new PointF(-1/2f, 1/2f), Vector.Midpoint(p1, p0), "P1 to P0 midpoint should be (-\u00BD,\u00BD)");
			Assert.AreEqual(new PointF(1/2f, -1/2f), Vector.Midpoint(p0, p2), "P0 to P2 midpoint should be (\u00BD,-\u00BD)");
			Assert.AreEqual(new PointF(1/2f, -1/2f), Vector.Midpoint(p2, p0), "P2 to P0 midpoint should be (\u00BD,-\u00BD)");
			Assert.AreEqual(new PointF(-1/2f, -1/2f), Vector.Midpoint(p0, p3), "P0 to P3 midpoint should be (-\u00BD,-\u00BD)");
			Assert.AreEqual(new PointF(-1/2f, -1/2f), Vector.Midpoint(p3, p0), "P3 to P0 midpoint should be (-\u00BD,-\u00BD)");

			// exercise cross-quadrant computations
			Assert.AreEqual(new PointF(0, 0), Vector.Midpoint(p1, p2), "P1 to P2 midpoint should be (0,0)");
			Assert.AreEqual(new PointF(0, 0), Vector.Midpoint(p2, p1), "P2 to P1 midpoint should be (0,0)");
			Assert.AreEqual(new PointF(-1, 0), Vector.Midpoint(p1, p3), "P1 to P3 midpoint should be (-1,0)");
			Assert.AreEqual(new PointF(-1, 0), Vector.Midpoint(p3, p1), "P3 to P1 midpoint should be (-1,0)");
			Assert.AreEqual(new PointF(0, -1), Vector.Midpoint(p2, p3), "P2 to P3 midpoint should be (0,-1)");
			Assert.AreEqual(new PointF(0, -1), Vector.Midpoint(p3, p2), "P3 to P2 midpoint should be (0,-1)");

			// exercise same-quadrant computation
			Assert.AreEqual(new PointF(root2F/2, -root2F/2), Vector.Midpoint(p0, new PointF(root2F, -root2F)), "P0 to (\u221A2,-\u221A2) midpoint should be (-\u221A2/2,-\u221A2/2)");
			Assert.AreEqual(new PointF(root2F/2, -root2F/2), Vector.Midpoint(new PointF(root2F, -root2F), p0), "(\u221A2,-\u221A2) to P0 midpoint should be (-\u221A2/2,-\u221A2/2)");
			Assert.AreEqual(new PointF(-1.5f, -1.5f), Vector.Midpoint(p3, p4), "P3 to P4 midpoint should be (-1\u00BD,-1\u00BD)");
			Assert.AreEqual(new PointF(-1.5f, -1.5f), Vector.Midpoint(p4, p3), "P4 to P3 midpoint should be (-1\u00BD,-1\u00BD)");
		}

		[Test]
		public void TestPointToLine()
		{
			var root2 = Math.Sqrt(2);
			var root3 = Math.Sqrt(3);
			var root5 = Math.Sqrt(5);
			var p0 = new PointF(0, 0);
			var p1 = new PointF(-1, -1);
			var p2 = new PointF(2, -1);
			var p3 = new PointF(1, 1);
			var p4 = new PointF(1, (float) root3);

			// exercise coincident point-point computations
			AssertPointToLine(0, p0, p0, p0, p0, "Point P0 and line P0P0 (coincident points))");
			AssertPointToLine(0, p1, p1, p1, p1, "Point P1 and line P1P1 (coincident points)");
			AssertPointToLine(0, p2, p2, p2, p2, "Point P2 and line P2P2 (coincident points)");
			AssertPointToLine(0, p3, p3, p3, p3, "Point P3 and line P3P3 (coincident points)");

			// exercise point-point computations
			AssertPointToLine(root2, p1, p0, p1, p1, "Point P0 and line P1P1 (point to point)");
			AssertPointToLine(3, p2, p1, p2, p2, "Point P1 and line P2P2 (point to point)");
			AssertPointToLine(root5, p3, p2, p3, p3, "Point P2 and line P3P3 (point to point)");
			AssertPointToLine(root2, p0, p3, p0, p0, "Point P3 and line P0P0 (point to point)");

			// exercise colinear point-line computations
			AssertPointToLine(0, p0, p0, p1, p3, "Point P0 and line P1P3 (colinear point to line segment)");
			AssertPointToLine(0, p0, p0, p3, p1, "Point P0 and line P3P1 (colinear point to line segment)");
			AssertPointToLine(root2, p0, p1, p0, p3, "Point P1 and line P0P3 (colinear point to line segment)");
			AssertPointToLine(root2, p0, p1, p3, p0, "Point P1 and line P3P0 (colinear point to line segment)");
			AssertPointToLine(root2, p0, p3, p0, p1, "Point P3 and line P0P1 (colinear point to line segment)");
			AssertPointToLine(root2, p0, p3, p1, p0, "Point P3 and line P1P0 (colinear point to line segment)");

			// exercise point-line computations
			AssertPointToLine(root2, p0, p1, p0, p2, "Point P1 and line P0P2 (point to line segment)");
			AssertPointToLine(root2, p0, p1, p2, p0, "Point P1 and line P2P0 (point to line segment)");
			AssertPointToLine(2, new PointF(1, -1), p3, p1, p2, "Point P3 and line P1P2 (point to line segment)");
			AssertPointToLine(2, new PointF(1, -1), p3, p2, p1, "Point P3 and line P2P1 (point to line segment)");
			AssertPointToLine(2*root3 - 1.5543624e-8, p4, new PointF(4, 0), p0, p4, "Point (4,0) and line P0P4 (point to line segment)");
			AssertPointToLine(2*root3 - 1.5543624e-8, p4, new PointF(4, 0), p4, p0, "Point (4,0) and line P4P0 (point to line segment)");
			// distance is off by that much here even when using Vector.Distance as a reference
		}

		private static void AssertPointToLine(double expectedDistance, PointF expectedPoint, PointF pT, PointF p1, PointF p2, string message, params object[] args)
		{
			var actualPoint = new PointF();
			var actualDistance = Vector.DistanceFromPointToLine(pT, p1, p2, ref actualPoint);
			Assert.AreEqual(expectedDistance, actualDistance, message, args);
			Assert.AreEqual(expectedPoint, actualPoint, message, args);
		}

		[Test]
		public void TestLegacyLineSegmentIntersection()
		{
			var p0 = new PointF(0, 0);
			var p1 = new PointF(3, 0);
			var p2 = new PointF(2, 1);
			var p3 = new PointF(1.5f, 0);
			var p4 = new PointF(1, -1);
			var p5 = new PointF(4, 0);
			var p6 = new PointF(2.5f, 2);
			var p7 = new PointF(5, 1);

			// exercise dual degenerate intersection computations
			AssertLineSegmentIntersection(Vector.LineSegments.Colinear, null, p0, p0, p0, p0, "P0P0 and P0P0 (point and point)");
			AssertLineSegmentIntersection(Vector.LineSegments.Colinear, null, p1, p1, p1, p1, "P1P1 and P1P1 (point and point)");
			AssertLineSegmentIntersection(Vector.LineSegments.Colinear, null, p0, p0, p1, p1, "P0P0 and P1P1 (point and point)");
			AssertLineSegmentIntersection(Vector.LineSegments.Colinear, null, p1, p1, p0, p0, "P1P1 and P0P0 (point and point)");

			// exercise single degenerate intersection computations
			AssertLineSegmentIntersection(Vector.LineSegments.DoNotIntersect, null, p2, p2, p0, p1, "P2P2 and P0P1 (point and line segment)");
			AssertLineSegmentIntersection(Vector.LineSegments.DoNotIntersect, null, p2, p2, p1, p0, "P2P2 and P1P0 (point and line segment)");
			AssertLineSegmentIntersection(Vector.LineSegments.Colinear, null, p3, p3, p0, p1, "P3P3 and P0P1 (point and line segment)");
			AssertLineSegmentIntersection(Vector.LineSegments.Colinear, null, p3, p3, p1, p0, "P3P3 and P1P0 (point and line segment)");
			AssertLineSegmentIntersection(Vector.LineSegments.Colinear, null, p5, p5, p0, p1, "P5P5 and P0P1 (point and line segment)");
			AssertLineSegmentIntersection(Vector.LineSegments.Colinear, null, p5, p5, p1, p0, "P5P5 and P1P0 (point and line segment)");

			// exercise two non-colinear line segments
			AssertLineSegmentIntersection(Vector.LineSegments.Intersect, p3, p2, p4, p0, p1, "P2P4 and P0P1 (distinct line segments)");
			AssertLineSegmentIntersection(Vector.LineSegments.Intersect, p3, p4, p2, p0, p1, "P4P2 and P0P1 (distinct line segments)");
			AssertLineSegmentIntersection(Vector.LineSegments.DoNotIntersect, null, p2, p6, p0, p1, "P2P6 and P0P1 (distinct line segments)");
			AssertLineSegmentIntersection(Vector.LineSegments.DoNotIntersect, null, p6, p2, p0, p1, "P6P2 and P0P1 (distinct line segments)");
			AssertLineSegmentIntersection(Vector.LineSegments.DoNotIntersect, null, p2, p7, p0, p1, "P2P7 and P0P1 (distinct line segments)");
			AssertLineSegmentIntersection(Vector.LineSegments.DoNotIntersect, null, p7, p2, p0, p1, "P7P2 and P0P1 (distinct line segments)");
			AssertLineSegmentIntersection(Vector.LineSegments.DoNotIntersect, null, p6, p7, p0, p1, "P6P7 and P0P1 (distinct line segments)");
			AssertLineSegmentIntersection(Vector.LineSegments.DoNotIntersect, null, p7, p6, p0, p1, "P7P6 and P0P1 (distinct line segments)");

			// exercise two colinear line segments
			AssertLineSegmentIntersection(Vector.LineSegments.Colinear, null, p0, p1, p3, p5, "P0P1 and P3P5 (partially overlapping line segments)");
			AssertLineSegmentIntersection(Vector.LineSegments.Colinear, null, p0, p1, p5, p3, "P0P1 and P5P3 (partially overlapping line segments)");
			AssertLineSegmentIntersection(Vector.LineSegments.Colinear, null, p1, p0, p3, p5, "P1P0 and P3P5 (partially overlapping line segments)");
			AssertLineSegmentIntersection(Vector.LineSegments.Colinear, null, p1, p0, p5, p3, "P1P0 and P5P3 (partially overlapping line segments)");
			AssertLineSegmentIntersection(Vector.LineSegments.Colinear, null, p0, p5, p3, p1, "P0P5 and P3P1 (overlapping line segments)");
			AssertLineSegmentIntersection(Vector.LineSegments.Colinear, null, p0, p5, p1, p3, "P0P5 and P1P3 (overlapping line segments)");
			AssertLineSegmentIntersection(Vector.LineSegments.Colinear, null, p3, p1, p5, p0, "P3P1 and P5P0 (overlapping line segments)");
			AssertLineSegmentIntersection(Vector.LineSegments.Colinear, null, p1, p3, p5, p0, "P1P3 and P5P0 (overlapping line segments)");
			AssertLineSegmentIntersection(Vector.LineSegments.Colinear, null, p0, p3, p1, p5, "P0P3 and P1P5 (distinct line segments)");
			AssertLineSegmentIntersection(Vector.LineSegments.Colinear, null, p3, p0, p5, p1, "P3P0 and P5P1 (distinct line segments)");
			AssertLineSegmentIntersection(Vector.LineSegments.Colinear, null, p2, p4, p2, p6, "P2P4 and P2P6 (common vertex line segments)");
			AssertLineSegmentIntersection(Vector.LineSegments.Colinear, null, p2, p4, p6, p2, "P2P4 and P6P2 (common vertex line segments)");
			AssertLineSegmentIntersection(Vector.LineSegments.Colinear, null, p4, p2, p2, p6, "P4P2 and P2P6 (common vertex line segments)");
			AssertLineSegmentIntersection(Vector.LineSegments.Colinear, null, p4, p2, p6, p2, "P4P2 and P6P2 (common vertex line segments)");
		}

		[Test]
		public void TestLineSegmentIntersection()
		{
			var p0 = new PointF(0, 0);
			var p1 = new PointF(3, 0);
			var p2 = new PointF(2, 1);
			var p3 = new PointF(1.5f, 0);
			var p4 = new PointF(1, -1);
			var p5 = new PointF(4, 0);
			var p6 = new PointF(2.5f, 2);
			var p7 = new PointF(5, 1);

			// exercise dual degenerate intersection computations
			AssertLineSegmentIntersection(null, p0, p0, p0, p0, "P0P0 and P0P0 (point and point)");
			AssertLineSegmentIntersection(null, p1, p1, p1, p1, "P1P1 and P1P1 (point and point)");
			AssertLineSegmentIntersection(null, p0, p0, p1, p1, "P0P0 and P1P1 (point and point)");
			AssertLineSegmentIntersection(null, p1, p1, p0, p0, "P1P1 and P0P0 (point and point)");

			// exercise single degenerate intersection computations
			AssertLineSegmentIntersection(null, p2, p2, p0, p1, "P2P2 and P0P1 (point and line segment)");
			AssertLineSegmentIntersection(null, p2, p2, p1, p0, "P2P2 and P1P0 (point and line segment)");
			AssertLineSegmentIntersection(null, p3, p3, p0, p1, "P3P3 and P0P1 (point and line segment)");
			AssertLineSegmentIntersection(null, p3, p3, p1, p0, "P3P3 and P1P0 (point and line segment)");
			AssertLineSegmentIntersection(null, p5, p5, p0, p1, "P5P5 and P0P1 (point and line segment)");
			AssertLineSegmentIntersection(null, p5, p5, p1, p0, "P5P5 and P1P0 (point and line segment)");

			// exercise two non-colinear line segments
			AssertLineSegmentIntersection(p3, p2, p4, p0, p1, "P2P4 and P0P1 (distinct line segments)");
			AssertLineSegmentIntersection(p3, p4, p2, p0, p1, "P4P2 and P0P1 (distinct line segments)");
			AssertLineSegmentIntersection(null, p2, p6, p0, p1, "P2P6 and P0P1 (distinct line segments)");
			AssertLineSegmentIntersection(null, p6, p2, p0, p1, "P6P2 and P0P1 (distinct line segments)");
			AssertLineSegmentIntersection(null, p2, p7, p0, p1, "P2P7 and P0P1 (distinct line segments)");
			AssertLineSegmentIntersection(null, p7, p2, p0, p1, "P7P2 and P0P1 (distinct line segments)");
			AssertLineSegmentIntersection(null, p6, p7, p0, p1, "P6P7 and P0P1 (distinct line segments)");
			AssertLineSegmentIntersection(null, p7, p6, p0, p1, "P7P6 and P0P1 (distinct line segments)");

			// exercise two colinear line segments
			AssertLineSegmentIntersection(null, p0, p1, p3, p5, "P0P1 and P3P5 (partially overlapping line segments)");
			AssertLineSegmentIntersection(null, p0, p1, p5, p3, "P0P1 and P5P3 (partially overlapping line segments)");
			AssertLineSegmentIntersection(null, p1, p0, p3, p5, "P1P0 and P3P5 (partially overlapping line segments)");
			AssertLineSegmentIntersection(null, p1, p0, p5, p3, "P1P0 and P5P3 (partially overlapping line segments)");
			AssertLineSegmentIntersection(null, p0, p5, p3, p1, "P0P5 and P3P1 (overlapping line segments)");
			AssertLineSegmentIntersection(null, p0, p5, p1, p3, "P0P5 and P1P3 (overlapping line segments)");
			AssertLineSegmentIntersection(null, p3, p1, p5, p0, "P3P1 and P5P0 (overlapping line segments)");
			AssertLineSegmentIntersection(null, p1, p3, p5, p0, "P1P3 and P5P0 (overlapping line segments)");
			AssertLineSegmentIntersection(null, p0, p3, p1, p5, "P0P3 and P1P5 (distinct line segments)");
			AssertLineSegmentIntersection(null, p3, p0, p5, p1, "P3P0 and P5P1 (distinct line segments)");
			AssertLineSegmentIntersection(null, p2, p4, p2, p6, "P2P4 and P2P6 (common vertex line segments)");
			AssertLineSegmentIntersection(null, p2, p4, p6, p2, "P2P4 and P6P2 (common vertex line segments)");
			AssertLineSegmentIntersection(null, p4, p2, p2, p6, "P4P2 and P2P6 (common vertex line segments)");
			AssertLineSegmentIntersection(null, p4, p2, p6, p2, "P4P2 and P6P2 (common vertex line segments)");
		}

		private static void AssertLineSegmentIntersectionP(Vector.LineSegments expectedResult, PointF? expectedIntersection, PointF p1, PointF p2, PointF q1, PointF q2, string message, params object[] args)
		{
			AssertLineSegmentIntersection(expectedResult, expectedIntersection, p1, p2, q1, q2, string.Format(message, args) + " [Permutation {0}]", "p1p2,q1q2");
			AssertLineSegmentIntersection(expectedResult, expectedIntersection, p2, p1, q1, q2, string.Format(message, args) + " [Permutation {0}]", "p2p1,q1q2");
			AssertLineSegmentIntersection(expectedResult, expectedIntersection, p1, p2, q2, q1, string.Format(message, args) + " [Permutation {0}]", "p1p2,q2q1");
			AssertLineSegmentIntersection(expectedResult, expectedIntersection, p2, p1, q2, q1, string.Format(message, args) + " [Permutation {0}]", "p2p1,q2q1");
			AssertLineSegmentIntersection(expectedResult, expectedIntersection, q1, q2, p1, p2, string.Format(message, args) + " [Permutation {0}]", "q1q2,p1p2");
			AssertLineSegmentIntersection(expectedResult, expectedIntersection, q2, q1, p1, p2, string.Format(message, args) + " [Permutation {0}]", "q2q1,p1p2");
			AssertLineSegmentIntersection(expectedResult, expectedIntersection, q1, q2, p2, p1, string.Format(message, args) + " [Permutation {0}]", "q1q2,p2p1");
			AssertLineSegmentIntersection(expectedResult, expectedIntersection, q2, q1, p2, p1, string.Format(message, args) + " [Permutation {0}]", "q2q1,p2p1");
		}

		private static void AssertLineSegmentIntersection(Vector.LineSegments expectedResult, PointF? expectedIntersection, PointF p1, PointF p2, PointF q1, PointF q2, string message, params object[] args)
		{
			// these checks protect against errors in the test case
			Platform.CheckFalse(expectedResult != Vector.LineSegments.Intersect && expectedIntersection.HasValue, "Expected intersection point must be null if not expecting an intersection.");
			Platform.CheckFalse(expectedResult == Vector.LineSegments.Intersect && !expectedIntersection.HasValue, "Expected intersection point must be provided if expecting an intersection.");

			PointF actualIntersection;
			var actualResult = Vector.LineSegmentIntersection(p1, p2, q1, q2, out actualIntersection);
			Assert.AreEqual(expectedResult, actualResult, message, args);

			if (actualResult == Vector.LineSegments.Intersect)
			{
				Console.WriteLine(" Expected: {0}", expectedIntersection.Value);
				Console.WriteLine("   Actual: {0}", actualIntersection);
				Assert.IsTrue(Math.Abs(expectedIntersection.Value.X - actualIntersection.X) <= 0.5001f, message, args);
				Assert.IsTrue(Math.Abs(expectedIntersection.Value.Y - actualIntersection.Y) <= 0.5001f, message, args);
				//Assert.AreEqual(expectedIntersection, actualIntersection, message, args);
			}
			else
			{
				Assert.AreEqual(actualIntersection, PointF.Empty, message, args);
			}
		}

		private static void AssertLineSegmentIntersection(PointF? expectedIntersection, PointF p1, PointF p2, PointF q1, PointF q2, string message, params object[] args)
		{
			PointF actualIntersection;
			var result = Vector.ComputeLineSegmentIntersection(p1, p2, q1, q2, out actualIntersection);
			if (expectedIntersection.HasValue)
			{
				Assert.IsTrue(result, message, args);
				//Assert.AreEqual(expectedIntersection.Value, actualIntersection, message, args);

				Console.WriteLine(" Expected: {0}", expectedIntersection.Value);
				Console.WriteLine("   Actual: {0}", actualIntersection);
				Assert.IsTrue(Math.Abs(expectedIntersection.Value.X - actualIntersection.X) <= 0.5001f, message, args);
				Assert.IsTrue(Math.Abs(expectedIntersection.Value.Y - actualIntersection.Y) <= 0.5001f, message, args);
			}
			else
			{
				Assert.IsFalse(result, message, args);
				Assert.AreEqual(PointF.Empty, actualIntersection, message, args);
			}
		}

		[Test]
		public void TestAreColinear()
		{
			var p0 = new PointF(0, 0);
			var p1 = new PointF(3, 0);
			var p2 = new PointF(2, 1);
			var p3 = new PointF(1.5f, 0);
			var p4 = new PointF(1, -1);
			var p5 = new PointF(4, 0);
			var p6 = new PointF(2.5f, 2);
			var p7 = new PointF(5, 1);

			// exercise dual degenerate inputs
			Assert.AreEqual(true, Vector.AreColinear(p0, p0, p0, p0), "P0P0 and P0P0 (point and point)");
			Assert.AreEqual(true, Vector.AreColinear(p1, p1, p1, p1), "P1P1 and P1P1 (point and point)");
			Assert.AreEqual(true, Vector.AreColinear(p0, p0, p1, p1), "P0P0 and P1P1 (point and point)");
			Assert.AreEqual(true, Vector.AreColinear(p1, p1, p0, p0), "P1P1 and P0P0 (point and point)");

			// exercise single degenerate inputs
			Assert.AreEqual(false, Vector.AreColinear(p2, p2, p0, p1), "P2P2 and P0P1 (point and line)");
			Assert.AreEqual(false, Vector.AreColinear(p2, p2, p1, p0), "P2P2 and P1P0 (point and line)");
			Assert.AreEqual(true, Vector.AreColinear(p3, p3, p0, p1), "P3P3 and P0P1 (point and line)");
			Assert.AreEqual(true, Vector.AreColinear(p3, p3, p1, p0), "P3P3 and P1P0 (point and line)");
			Assert.AreEqual(true, Vector.AreColinear(p5, p5, p0, p1), "P5P5 and P0P1 (point and line)");
			Assert.AreEqual(true, Vector.AreColinear(p5, p5, p1, p0), "P5P5 and P1P0 (point and line)");

			// exercise two non-colinear lines
			Assert.AreEqual(false, Vector.AreColinear(p2, p4, p0, p1), "P2P4 and P0P1 (distinct lines)");
			Assert.AreEqual(false, Vector.AreColinear(p4, p2, p0, p1), "P4P2 and P0P1 (distinct lines)");
			Assert.AreEqual(false, Vector.AreColinear(p2, p6, p0, p1), "P2P6 and P0P1 (distinct lines)");
			Assert.AreEqual(false, Vector.AreColinear(p6, p2, p0, p1), "P6P2 and P0P1 (distinct lines)");
			Assert.AreEqual(false, Vector.AreColinear(p2, p7, p0, p1), "P2P7 and P0P1 (distinct lines)");
			Assert.AreEqual(false, Vector.AreColinear(p7, p2, p0, p1), "P7P2 and P0P1 (distinct lines)");
			Assert.AreEqual(false, Vector.AreColinear(p6, p7, p0, p1), "P6P7 and P0P1 (distinct lines)");
			Assert.AreEqual(false, Vector.AreColinear(p7, p6, p0, p1), "P7P6 and P0P1 (distinct lines)");

			// exercise two colinear lines
			Assert.AreEqual(true, Vector.AreColinear(p0, p1, p3, p5), "P0P1 and P3P5 (partially overlapping lines)");
			Assert.AreEqual(true, Vector.AreColinear(p0, p1, p5, p3), "P0P1 and P5P3 (partially overlapping lines)");
			Assert.AreEqual(true, Vector.AreColinear(p1, p0, p3, p5), "P1P0 and P3P5 (partially overlapping lines)");
			Assert.AreEqual(true, Vector.AreColinear(p1, p0, p5, p3), "P1P0 and P5P3 (partially overlapping lines)");
			Assert.AreEqual(true, Vector.AreColinear(p0, p5, p3, p1), "P0P5 and P3P1 (overlapping lines)");
			Assert.AreEqual(true, Vector.AreColinear(p0, p5, p1, p3), "P0P5 and P1P3 (overlapping lines)");
			Assert.AreEqual(true, Vector.AreColinear(p3, p1, p5, p0), "P3P1 and P5P0 (overlapping lines)");
			Assert.AreEqual(true, Vector.AreColinear(p1, p3, p5, p0), "P1P3 and P5P0 (overlapping lines)");
			Assert.AreEqual(true, Vector.AreColinear(p0, p3, p1, p5), "P0P3 and P1P5 (distinct lines)");
			Assert.AreEqual(true, Vector.AreColinear(p3, p0, p5, p1), "P3P0 and P5P1 (distinct lines)");
			Assert.AreEqual(true, Vector.AreColinear(p2, p4, p2, p6), "P2P4 and P2P6 (common vertex lines)");
			Assert.AreEqual(true, Vector.AreColinear(p2, p4, p6, p2), "P2P4 and P6P2 (common vertex lines)");
			Assert.AreEqual(true, Vector.AreColinear(p4, p2, p2, p6), "P4P2 and P2P6 (common vertex lines)");
			Assert.AreEqual(true, Vector.AreColinear(p4, p2, p6, p2), "P4P2 and P6P2 (common vertex lines)");
		}
	}
}

#endif