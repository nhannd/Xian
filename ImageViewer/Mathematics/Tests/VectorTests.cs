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
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Mathematics.Tests
{
	[TestFixture]
	public class VectorTests
	{
		public VectorTests()
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
		public void SubtendedAngle()
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
		public void UnitVector()
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
	}
}

#endif