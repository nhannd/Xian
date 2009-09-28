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
using System.Drawing.Drawing2D;
using ClearCanvas.ImageViewer.Graphics;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.RoiGraphics.Tests
{
	[TestFixture]
	public class RectangularRoiTests : RoiTestBase<RectangleF>
	{
		[Test]
		public void TestContains()
		{
			RectangleF rectangle = new RectangleF(77, 79, 100, 100);
			base.TestRoiContains(ImageKey.Simple01, rectangle, null);
		}

		[Test]
		public void TestStatsCalculationSimple01()
		{
			// these expected values were independently computed by hand
			RectangleF rectangle = new RectangleF(77, 79, 100, 100);
			base.TestRoiStatsCalculations(ImageKey.Simple01, rectangle, 400, 10000, 255, 0);
		}

		[Test]
		public void TestStatsCalculationSimple02()
		{
			// these expected values were independently computed by hand
			RectangleF rectangle = new RectangleF(77, 79, 100, 100);
			base.TestRoiStatsCalculations(ImageKey.Simple02, rectangle, 400, 10000, 128.3, 73.98);
		}

		[Test]
		public void TestStatsCalculationSimple03()
		{
			// these expected values were independently computed by hand
			RectangleF rectangle = new RectangleF(77, 79, 100, 100);
			base.TestRoiStatsCalculations(ImageKey.Simple03, rectangle, 400, 10000, 127.9, 52.05);
		}

		[Test]
		public void TestStatsCalculationsConsistency()
		{
			base.TestRoiStatsCalculationConsistency();
		}

		protected override RectangleF CreateCoreSampleShape(PointF location, int imageRows, int imageCols)
		{
			const int s = 10;
			return RectangleF.FromLTRB(location.X, location.Y, Math.Min(imageCols - 1, location.X + s), Math.Min(imageRows - 1, location.Y + s));
		}

		protected override Roi CreateRoiFromGraphic(IOverlayGraphicsProvider overlayGraphics, RectangleF shapeData)
		{
			RectanglePrimitive graphic = new RectanglePrimitive();
			graphic.CoordinateSystem = CoordinateSystem.Source;
			graphic.TopLeft = shapeData.Location;
			graphic.BottomRight = shapeData.Location + shapeData.Size;
			graphic.ResetCoordinateSystem();
			overlayGraphics.OverlayGraphics.Add(graphic);
			return graphic.GetRoi();
		}

		protected override Roi CreateRoiFromImage(IPresentationImage image, RectangleF shapeData)
		{
			return new RectangularRoi(shapeData, image);
		}

		protected override void AddShapeToGraphicsPath(GraphicsPath graphicsPath, RectangleF shapeData)
		{
			graphicsPath.AddRectangle(shapeData);
		}

		protected override string ShapeName
		{
			get { return "rectangle"; }
		}
	}
}

#endif