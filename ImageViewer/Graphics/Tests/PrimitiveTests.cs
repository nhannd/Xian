﻿#region License

// Copyright (c) 2010, ClearCanvas Inc.
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

namespace ClearCanvas.ImageViewer.Graphics.Tests
{
	[TestFixture]
	public class PrimitiveTests
	{
		private const float _dimensionTolerance = 0.0001f;

		[Test]
		public void TestRectangleGetRoi()
		{
			// test null rectangle
			{
				var rect = new RectanglePrimitive();
				rect.TopLeft = new PointF(5, 5);
				rect.BottomRight = new PointF(5, 5);
				var roi = rect.GetRoi();
				Assert.IsNotNull(roi, "RectanglePrimitive.GetRoi() should never return null (rect = {0})", rect.Rectangle);
				AssertAreEqual(new RectangleF(5, 5, 0, 0), roi.BoundingBox, _dimensionTolerance, "RectanglePrimitive.GetRoi() returned wrong ROI (rect = {0})", rect.Rectangle);
			}

			// test positive rectangle
			{
				var rect = new RectanglePrimitive();
				rect.TopLeft = new PointF(0, 0);
				rect.BottomRight = new PointF(1, 1);
				var roi = rect.GetRoi();
				Assert.IsNotNull(roi, "RectanglePrimitive.GetRoi() should never return null (rect = {0})", rect.Rectangle);
				AssertAreEqual(new RectangleF(0, 0, 1, 1), roi.BoundingBox, _dimensionTolerance, "RectanglePrimitive.GetRoi() returned wrong ROI (rect = {0})", rect.Rectangle);
			}

			// test inverted rectangle
			{
				var rect = new RectanglePrimitive();
				rect.TopLeft = new PointF(5, 5);
				rect.BottomRight = new PointF(-1, -1);
				var roi = rect.GetRoi();
				Assert.IsNotNull(roi, "RectanglePrimitive.GetRoi() should never return null (rect = {0})", rect.Rectangle);
				AssertAreEqual(new RectangleF(-1, -1, 6, 6), roi.BoundingBox, _dimensionTolerance, "RectanglePrimitive.GetRoi() returned wrong ROI (rect = {0})", rect.Rectangle);
			}

			// test negative width rectangle
			{
				var rect = new RectanglePrimitive();
				rect.TopLeft = new PointF(5, 5);
				rect.BottomRight = new PointF(-1, 6);
				var roi = rect.GetRoi();
				Assert.IsNotNull(roi, "RectanglePrimitive.GetRoi() should never return null (rect = {0})", rect.Rectangle);
				AssertAreEqual(new RectangleF(-1, 5, 6, 1), roi.BoundingBox, _dimensionTolerance, "RectanglePrimitive.GetRoi() returned wrong ROI (rect = {0})", rect.Rectangle);
			}

			// test negative height rectangle
			{
				var rect = new RectanglePrimitive();
				rect.TopLeft = new PointF(5, 5);
				rect.BottomRight = new PointF(6, -1);
				var roi = rect.GetRoi();
				Assert.IsNotNull(roi, "RectanglePrimitive.GetRoi() should never return null (rect = {0})", rect.Rectangle);
				AssertAreEqual(new RectangleF(5, -1, 1, 6), roi.BoundingBox, _dimensionTolerance, "RectanglePrimitive.GetRoi() returned wrong ROI (rect = {0})", rect.Rectangle);
			}
		}

		[Test]
		public void TestEllipseGetRoi()
		{
			// test null ellipse
			{
				var ellipse = new EllipsePrimitive();
				ellipse.TopLeft = new PointF(5, 5);
				ellipse.BottomRight = new PointF(5, 5);
				var roi = ellipse.GetRoi();
				Assert.IsNotNull(roi, "EllipsePrimitive.GetRoi() should never return null (rect = {0})", ellipse.Rectangle);
				AssertAreEqual(new RectangleF(5, 5, 0, 0), roi.BoundingBox, _dimensionTolerance, "EllipsePrimitive.GetRoi() returned wrong ROI (ellipse = {0})", ellipse.Rectangle);
			}

			// test positive ellipse
			{
				var ellipse = new EllipsePrimitive();
				ellipse.TopLeft = new PointF(0, 0);
				ellipse.BottomRight = new PointF(1, 1);
				var roi = ellipse.GetRoi();
				Assert.IsNotNull(roi, "EllipsePrimitive.GetRoi() should never return null (rect = {0})", ellipse.Rectangle);
				AssertAreEqual(new RectangleF(0, 0, 1, 1), roi.BoundingBox, _dimensionTolerance, "EllipsePrimitive.GetRoi() returned wrong ROI (ellipse = {0})", ellipse.Rectangle);
			}

			// test inverted ellipse
			{
				var ellipse = new EllipsePrimitive();
				ellipse.TopLeft = new PointF(5, 5);
				ellipse.BottomRight = new PointF(-1, -1);
				var roi = ellipse.GetRoi();
				Assert.IsNotNull(roi, "EllipsePrimitive.GetRoi() should never return null (rect = {0})", ellipse.Rectangle);
				AssertAreEqual(new RectangleF(-1, -1, 6, 6), roi.BoundingBox, _dimensionTolerance, "EllipsePrimitive.GetRoi() returned wrong ROI (ellipse = {0})", ellipse.Rectangle);
			}

			// test negative width ellipse
			{
				var ellipse = new EllipsePrimitive();
				ellipse.TopLeft = new PointF(5, 5);
				ellipse.BottomRight = new PointF(-1, 6);
				var roi = ellipse.GetRoi();
				Assert.IsNotNull(roi, "EllipsePrimitive.GetRoi() should never return null (rect = {0})", ellipse.Rectangle);
				AssertAreEqual(new RectangleF(-1, 5, 6, 1), roi.BoundingBox, _dimensionTolerance, "EllipsePrimitive.GetRoi() returned wrong ROI (ellipse = {0})", ellipse.Rectangle);
			}

			// test negative height ellipse
			{
				var ellipse = new EllipsePrimitive();
				ellipse.TopLeft = new PointF(5, 5);
				ellipse.BottomRight = new PointF(6, -1);
				var roi = ellipse.GetRoi();
				Assert.IsNotNull(roi, "EllipsePrimitive.GetRoi() should never return null (rect = {0})", ellipse.Rectangle);
				AssertAreEqual(new RectangleF(5, -1, 1, 6), roi.BoundingBox, _dimensionTolerance, "EllipsePrimitive.GetRoi() returned wrong ROI (ellipse = {0})", ellipse.Rectangle);
			}
		}

		private static void AssertAreEqual(RectangleF expected, RectangleF actual, float delta, string message, params object[] args)
		{
			try
			{
				Assert.AreEqual(expected.Left, actual.Left, delta, message, args);
				Assert.AreEqual(expected.Right, actual.Right, delta, message, args);
				Assert.AreEqual(expected.Width, actual.Width, delta, message, args);
				Assert.AreEqual(expected.Height, actual.Height, delta, message, args);
			}
			catch (Exception)
			{
				Console.WriteLine("\tExpected: {0}", expected);
				Console.WriteLine("\t  Actual: {0}", actual);
				throw;
			}
		}
	}
}

#endif