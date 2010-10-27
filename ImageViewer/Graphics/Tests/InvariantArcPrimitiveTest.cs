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

using NUnit.Framework;
using System.Drawing;
using System;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Graphics.Tests
{
	[TestFixture]
	public class InvariantArcPrimitiveTest
	{
		public InvariantArcPrimitiveTest()
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
		public void CoordinateChange()
		{
			InvariantArcPrimitive arc = new InvariantArcPrimitive();

			for (int angle = 0; angle <= 360; angle += 90)
			{
				arc.SpatialTransform.RotationXY = angle;
				arc.SpatialTransform.FlipX = true;
				arc.SpatialTransform.FlipY = true;
				arc.CoordinateSystem = CoordinateSystem.Source;
				arc.StartAngle = 30;
				arc.CoordinateSystem = CoordinateSystem.Destination;
				float dstAngle = arc.StartAngle;
				arc.StartAngle = dstAngle;
				arc.CoordinateSystem = CoordinateSystem.Source;
				Assert.AreEqual(30, arc.StartAngle);
			}
		}
	}
}

#endif