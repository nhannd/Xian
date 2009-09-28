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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using ClearCanvas.ImageViewer.Graphics;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.RoiGraphics.Tests
{
	[TestFixture]
	public partial class PolygonalRoiTests : RoiTestBase<IEnumerable<PointF>>
	{
		[Test]
		public void TestConstruction()
		{
			PolygonalRoi r1 = new PolygonalRoi(Enumerate(new PointF(0, 0), new PointF(0, 0), new PointF(0, 0)), null);
			Assert.AreEqual(3, r1.Polygon.CountVertices);
			Assert.AreEqual(3, r1.Polygon.Vertices.Count);

			PolygonalRoi r2 = new PolygonalRoi(Enumerate(new PointF(0, 0), new PointF(1, 1), new PointF(0, 0)), null);
			Assert.AreEqual(3, r2.Polygon.CountVertices);
			Assert.AreEqual(3, r2.Polygon.Vertices.Count);

			PolygonalRoi r3 = new PolygonalRoi(Enumerate(new PointF(0, 0), new PointF(0, 1), new PointF(1, 1)), null);
			Assert.AreEqual(3, r3.Polygon.CountVertices);
			Assert.AreEqual(3, r3.Polygon.Vertices.Count);
		}

		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void TestInvalidConstruction()
		{
			new PolygonalRoi(Enumerate(new PointF(0, 0)), null);
		}

		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void TestInvalidConstruction2()
		{
			new PolygonalRoi(Enumerate(new PointF(0, 0), new PointF(0, 1)), null);
		}

		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void TestInvalidConstructionFromUnclosedPolyline()
		{
			PolylineGraphic plg = new PolylineGraphic();
			plg.Points.Add(new PointF(0, 0));
			plg.Points.Add(new PointF(0, 1));
			new PolygonalRoi(plg);
		}

		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void TestInvalidConstructionFromUnclosedPolyline2()
		{
			PolylineGraphic plg = new PolylineGraphic();
			plg.Points.Add(new PointF(0, 0));
			plg.Points.Add(new PointF(0, 1));
			plg.Points.Add(new PointF(1, 1));
			new PolygonalRoi(plg);
		}

		[Test]
		public void TestInvalidConstructionFromClosedPolyline()
		{
			PolylineGraphic plg = new PolylineGraphic();
			plg.Points.Add(new PointF(0, 0));
			Assert.IsNull(new PolygonalRoi(plg).Polygon);

			PolylineGraphic plg2 = new PolylineGraphic();
			plg2.Points.Add(new PointF(0, 0));
			plg2.Points.Add(new PointF(0, 0));
			Assert.IsNull(new PolygonalRoi(plg2).Polygon);

			PolylineGraphic plg3 = new PolylineGraphic();
			plg3.Points.Add(new PointF(0, 0));
			plg3.Points.Add(new PointF(1, 1));
			plg3.Points.Add(new PointF(0, 0));
			Assert.IsNull(new PolygonalRoi(plg3).Polygon);
		}

		[Test]
		public void TestConstructionFromPolyline()
		{
			PolylineGraphic plg = new PolylineGraphic();
			plg.Points.Add(new PointF(0, 0));
			plg.Points.Add(new PointF(0, 1));
			plg.Points.Add(new PointF(1, 1));
			plg.Points.Add(plg.Points[0]);
			PolygonalRoi roi = new PolygonalRoi(plg);
			Assert.IsNotNull(roi.Polygon);
			Assert.AreEqual(3, roi.Polygon.CountVertices);
			Assert.AreEqual(3, roi.Polygon.Vertices.Count);
			Assert.AreEqual(0.5, roi.Area);

			PolylineGraphic plg2 = new PolylineGraphic();
			plg2.Points.Add(new PointF(0, 0));
			plg2.Points.Add(new PointF(0, 1));
			plg2.Points.Add(new PointF(1, 1));
			plg2.Points.Add(new PointF(1, 0));
			plg2.Points.Add(plg2.Points[0]);
			PolygonalRoi roi2 = new PolygonalRoi(plg2);
			Assert.IsNotNull(roi2.Polygon);
			Assert.AreEqual(4, roi2.Polygon.CountVertices);
			Assert.AreEqual(4, roi2.Polygon.Vertices.Count);
			Assert.AreEqual(1, roi2.Area);

			PolylineGraphic plg3 = new PolylineGraphic();
			plg3.Points.Add(new PointF(0, 0));
			plg3.Points.Add(new PointF(0, 1000));
			plg3.Points.Add(new PointF(1000, 0));
			plg3.Points.Add(new PointF(1000, 1000));
			plg3.Points.Add(plg3.Points[0]);
			PolygonalRoi roi3 = new PolygonalRoi(plg3);
			Assert.IsNotNull(roi3.Polygon);
			Assert.AreEqual(4, roi3.Polygon.CountVertices);
			Assert.AreEqual(4, roi3.Polygon.Vertices.Count);
			Assert.AreEqual(500000, roi3.Area, 5000);
		}

		[Test]
		public void TestIsComplex()
		{
			using (IPresentationImage image = GetImage(ImageKey.Complex10))
			{
				Assert.IsFalse(new PolygonalRoi(PolygonShape.Sierra, image).Polygon.IsComplex, "Polygon complexity test false positive fail.");
				Assert.IsFalse(new PolygonalRoi(PolygonShape.Golf, image).Polygon.IsComplex, "Polygon complexity test false positive fail.");
				Assert.IsFalse(new PolygonalRoi(PolygonShape.Charlie, image).Polygon.IsComplex, "Polygon complexity test false positive fail.");
				Assert.IsFalse(new PolygonalRoi(PolygonShape.Arrowhead, image).Polygon.IsComplex, "Polygon complexity test false positive fail.");

				Assert.IsTrue(new PolygonalRoi(PolygonShape.SierraPrime, image).Polygon.IsComplex, "Polygon complexity test false negative fail.");
				Assert.IsTrue(new PolygonalRoi(PolygonShape.GolfPrime, image).Polygon.IsComplex, "Polygon complexity test false negative fail.");
				Assert.IsTrue(new PolygonalRoi(PolygonShape.CharliePrime, image).Polygon.IsComplex, "Polygon complexity test false negative fail.");
				Assert.IsTrue(new PolygonalRoi(PolygonShape.ArrowheadPrime, image).Polygon.IsComplex, "Polygon complexity test false negative fail.");
			}
		}

		[Test]
		public void TestContains()
		{
			base.TestRoiContains(ImageKey.Complex01, PolygonShape.Golf, "golf");
			base.TestRoiContains(ImageKey.Complex01, PolygonShape.GolfPrime, "golf_prime");
			base.TestRoiContains(ImageKey.Complex04, PolygonShape.Charlie, "charlie");
			base.TestRoiContains(ImageKey.Complex04, PolygonShape.CharliePrime, "charlie_prime");
			base.TestRoiContains(ImageKey.Complex07, PolygonShape.Sierra, "sierra");
			base.TestRoiContains(ImageKey.Complex07, PolygonShape.SierraPrime, "sierra_prime");
			base.TestRoiContains(ImageKey.Complex10, PolygonShape.Arrowhead, "arrowhead");
			base.TestRoiContains(ImageKey.Complex10, PolygonShape.ArrowheadPrime, "arrowhead_prime");
		}

		[Test]
		public void TestContains2()
		{
			WriteLine("The validation images output by the following tests should be compared against the ROI in the corresponding presentation state.");
			base.TestRoiContains(ImageKey.Real01, PolygonShape.ShapeOnReal01, "real01");
			base.TestRoiContains(ImageKey.Real02, PolygonShape.ShapeOnReal02, "real02");
			base.TestRoiContains(ImageKey.Real03, PolygonShape.ShapeOnReal03, "real03");
			base.TestRoiContains(ImageKey.Real04, PolygonShape.ShapeOnReal04, "real04");
			base.TestRoiContains(ImageKey.Real05, PolygonShape.ShapeOnReal05, "real05");
			base.TestRoiContains(ImageKey.Real06, PolygonShape.ShapeOnReal06, "real06");
		}

		[Test]
		public void TestContains3()
		{
			_useComplexCoreSampleShape = false;
			base.TestRoiContains(ImageKey.Complex01, CreateCoreSampleShape(new PointF(127, 127), 256, 256), "pentagon");
			base.TestRoiContains(ImageKey.Complex01, CreateCoreSampleShape(new PointF(127, 127), 256, 256), "5pointstar");
		}

		[Test]
		public void TestStatsCalculationComplexShapeSierra()
		{
			// this is the figure S
			// these expected values were independently computed by hand
			base.TestRoiStatsCalculations(ImageKey.Complex07, PolygonShape.Sierra, 1150, 13750, 255, 0);
			base.TestRoiStatsCalculations(ImageKey.Complex07, PolygonShape.SierraPrime, 1150, 13750, 255, 0);
			base.TestRoiStatsCalculations(ImageKey.Complex08, PolygonShape.Sierra, 1150, 13750, 159.7, 52.16);
			base.TestRoiStatsCalculations(ImageKey.Complex08, PolygonShape.SierraPrime, 1150, 13750, 159.7, 52.16);
			base.TestRoiStatsCalculations(ImageKey.Complex09, PolygonShape.Sierra, 1150, 13750, 222.6, 17.40);
			base.TestRoiStatsCalculations(ImageKey.Complex09, PolygonShape.SierraPrime, 1150, 13750, 222.6, 17.40);
		}

		[Test]
		public void TestStatsCalculationComplexShapeGolf()
		{
			// this is the figure G
			// these expected values were independently computed by hand
			base.TestRoiStatsCalculations(ImageKey.Complex01, PolygonShape.Golf, 1100, 13125, 255, 0);
			base.TestRoiStatsCalculations(ImageKey.Complex01, PolygonShape.GolfPrime, 1100, 13125, 255, 0);
			base.TestRoiStatsCalculations(ImageKey.Complex02, PolygonShape.Golf, 1100, 13125, 161.5, 53.77);
			base.TestRoiStatsCalculations(ImageKey.Complex02, PolygonShape.GolfPrime, 1100, 13125, 161.5, 53.77);
			base.TestRoiStatsCalculations(ImageKey.Complex03, PolygonShape.Golf, 1100, 13125, 225.2, 18.71);
			base.TestRoiStatsCalculations(ImageKey.Complex03, PolygonShape.GolfPrime, 1100, 13125, 225.2, 18.71);
		}

		[Test]
		public void TestStatsCalculationComplexShapeCharlie()
		{
			// this is the figure C
			// these expected values were independently computed by hand
			base.TestRoiStatsCalculations(ImageKey.Complex04, PolygonShape.Charlie, 1100, 16875, 255, 0);
			base.TestRoiStatsCalculations(ImageKey.Complex04, PolygonShape.CharliePrime, 1100, 16875, 255, 0);
			base.TestRoiStatsCalculations(ImageKey.Complex05, PolygonShape.Charlie, 1100, 16875, 165.2, 53.04);
			base.TestRoiStatsCalculations(ImageKey.Complex05, PolygonShape.CharliePrime, 1100, 16875, 165.2, 53.04);
			base.TestRoiStatsCalculations(ImageKey.Complex06, PolygonShape.Charlie, 1100, 16875, 228.8, 15.59);
			base.TestRoiStatsCalculations(ImageKey.Complex06, PolygonShape.CharliePrime, 1100, 16875, 228.8, 15.59);
		}

		[Test]
		public void TestStatsCalculationComplexShapeArrowhead()
		{
			// this is the arrowhead figure
			// these expected values were independently computed by ImageJ using the exact same polygon points
			base.TestRoiStatsCalculations(ImageKey.Complex10, PolygonShape.Arrowhead, 561.07, 9375, 255, 0);
			base.TestRoiStatsCalculations(ImageKey.Complex10, PolygonShape.ArrowheadPrime, 561.07, 9375, 255, 0);
			base.TestRoiStatsCalculations(ImageKey.Complex11, PolygonShape.Arrowhead, 561.07, 9375, 178.839, 51.206);
			base.TestRoiStatsCalculations(ImageKey.Complex11, PolygonShape.ArrowheadPrime, 561.07, 9375, 178.839, 51.206);
			base.TestRoiStatsCalculations(ImageKey.Complex12, PolygonShape.Arrowhead, 561.07, 9375, 223.276, 18.161);
			base.TestRoiStatsCalculations(ImageKey.Complex12, PolygonShape.ArrowheadPrime, 561.07, 9375, 223.276, 18.161);
		}

		[Test]
		public void TestStatsCalculationRealShapesOnRealImages()
		{
			// these expected values were independently computed by ImageJ using the exact same polygon points
			base.TestRoiStatsCalculations(ImageKey.Real01, PolygonShape.ShapeOnReal01, 93.495, 196, 78.944, 22.693);
			base.TestRoiStatsCalculations(ImageKey.Real02, PolygonShape.ShapeOnReal02, 113.552, 371, 77.879, 12.912);
			base.TestRoiStatsCalculations(ImageKey.Real03, PolygonShape.ShapeOnReal03, 193.873, 1341, 1.584, 13.858);
			base.TestRoiStatsCalculations(ImageKey.Real04, PolygonShape.ShapeOnReal04, 336.238, 3875, 18.09, 11.676);
			base.TestRoiStatsCalculations(ImageKey.Real05, PolygonShape.ShapeOnReal05, 215.188, 3123, 96.19, 9.741);
			base.TestRoiStatsCalculations(ImageKey.Real06, PolygonShape.ShapeOnReal06, 191.834, 1797, 214.337, 50.178);
		}

		[Test]
		public void TestStatsCalculationsConsistency()
		{
			base.TestRoiStatsCalculationConsistency();
		}

		private static IEnumerable<T> Enumerate<T>(params T[] args)
		{
			foreach (T arg in args)
				yield return arg;
		}

		private static T[] Arrayize<T>(IEnumerable<T> elements)
		{
			return new List<T>(elements).ToArray();
		}

		protected override string ShapeName
		{
			get { return "polygon"; }
		}

		private bool _useComplexCoreSampleShape = false;

		protected override IEnumerable<PointF> CreateCoreSampleShape(PointF location, int imageRows, int imageCols)
		{
			const int side = 10;
			SizeF offset = new SizeF(location);
			List<PointF> results = new List<PointF>();
			if (_useComplexCoreSampleShape)
			{
				// this makes a 5-point star
				for (int i = 0; i < 5; i++)
				{
					double angle = (-5 + 8*i)*Math.PI/10d;
					results.Add(new PointF(side * (float)Math.Cos(angle), side * (float)Math.Sin(angle)) + offset); 
				}
			}
			else
			{
				// this makes a pentagon
				for (int i = 0; i < 5; i++)
				{
					double angle = (-5 + 4*i)*Math.PI/10d;
					results.Add(new PointF(side*(float) Math.Cos(angle), side*(float) Math.Sin(angle)) + offset);
				}
			}
			_useComplexCoreSampleShape = !_useComplexCoreSampleShape;
			return results.AsReadOnly();
		}

		protected override Roi CreateRoiFromGraphic(IOverlayGraphicsProvider overlayGraphics, IEnumerable<PointF> shapeData)
		{
			PolylineGraphic graphic = new PolylineGraphic();
			graphic.CoordinateSystem = CoordinateSystem.Source;
			foreach (PointF data in shapeData)
				graphic.Points.Add(data);
			graphic.Points.Add(graphic.Points[0]);
			graphic.ResetCoordinateSystem();
			overlayGraphics.OverlayGraphics.Add(graphic);
			return graphic.GetRoi();
		}

		protected override Roi CreateRoiFromImage(IPresentationImage image, IEnumerable<PointF> shapeData)
		{
			return new PolygonalRoi(shapeData, image);
		}

		protected override void AddShapeToGraphicsPath(GraphicsPath graphicsPath, IEnumerable<PointF> shapeData)
		{
			graphicsPath.FillMode = FillMode.Winding;
			graphicsPath.AddPolygon(Arrayize(shapeData));
		}
	}
}

#endif