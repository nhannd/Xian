#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if	UNIT_TESTS
#pragma warning disable 1591,0419,1574,1587

using System.Collections.Generic;
using System.Drawing;
using System.Text;
using ClearCanvas.ImageViewer.RoiGraphics;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Graphics.Tests
{
	[TestFixture]
	public class PolylineGraphicTests
	{
		[Test]
		public void TestGetRoiFromNullShape()
		{
			var graphic = new PolylineGraphic();
			var roi = graphic.GetRoi();
			Assert.IsNull(roi, "PolylineGraphic.GetRoi() should return null for shape {0}", Format(graphic.Points));
		}

		[Test]
		public void TestGetRoiFromPoint()
		{
			var graphic = new PolylineGraphic();
			graphic.Points.Add(new PointF(0, 0));
			var roi = graphic.GetRoi();
			Assert.IsNull(roi, "PolylineGraphic.GetRoi() should return null for shape {0}", Format(graphic.Points));
		}

		[Test]
		public void TestGetRoiFromLineSegment()
		{
			var graphic = new PolylineGraphic();
			graphic.Points.Add(new PointF(0, 0));
			graphic.Points.Add(new PointF(0, 1));
			var roi = graphic.GetRoi();
			Assert.IsInstanceOf(typeof (LinearRoi), roi, "PolylineGraphic.GetRoi() should return LinearRoi for shape {0}", Format(graphic.Points));
		}

		[Test]
		public void TestGetRoiFromPolyline()
		{
			// test 3 point polyline (closed shape)
			{
				var graphic = new PolylineGraphic();
				graphic.Points.Add(new PointF(0, 0));
				graphic.Points.Add(new PointF(0, 1));
				graphic.Points.Add(new PointF(0, 0));
				var roi = graphic.GetRoi();
				Assert.IsNull(roi, "PolylineGraphic.GetRoi() should return null for shape {0}", Format(graphic.Points));
			}

			// test 3 point polyline (unclosed shape)
			{
				var graphic = new PolylineGraphic();
				graphic.Points.Add(new PointF(0, 0));
				graphic.Points.Add(new PointF(0, 1));
				graphic.Points.Add(new PointF(1, 1));
				var roi = graphic.GetRoi();
				Assert.IsNull(roi, "PolylineGraphic.GetRoi() should return null for shape {0}", Format(graphic.Points));
			}

			// test 4 point polyline (unclosed shape)
			{
				var graphic = new PolylineGraphic();
				graphic.Points.Add(new PointF(0, 0));
				graphic.Points.Add(new PointF(0, 1));
				graphic.Points.Add(new PointF(1, 1));
				graphic.Points.Add(new PointF(1, 2));
				var roi = graphic.GetRoi();
				Assert.IsNull(roi, "PolylineGraphic.GetRoi() should return null for shape {0}", Format(graphic.Points));
			}
		}

		[Test]
		public void TestGetRoiFromPolygon()
		{
			var graphic = new PolylineGraphic();
			graphic.Points.Add(new PointF(0, 0));
			graphic.Points.Add(new PointF(0, 1));
			graphic.Points.Add(new PointF(1, 1));
			graphic.Points.Add(graphic.Points[0]);
			var roi = graphic.GetRoi();
            Assert.IsInstanceOf(typeof(PolygonalRoi), roi, "PolylineGraphic.GetRoi() should return PolygonalRoi for shape {0}", Format(graphic.Points));

			var graphic2 = new PolylineGraphic();
			graphic2.Points.Add(new PointF(0, 0));
			graphic2.Points.Add(new PointF(0, 1));
			graphic2.Points.Add(new PointF(1, 1));
			graphic2.Points.Add(new PointF(1, 0));
			graphic2.Points.Add(graphic2.Points[0]);
			var roi2 = graphic2.GetRoi();
            Assert.IsInstanceOf(typeof(PolygonalRoi), roi2, "PolylineGraphic.GetRoi() should return PolygonalRoi for shape {0}", Format(graphic2.Points));

			var graphic3 = new PolylineGraphic();
			graphic3.Points.Add(new PointF(0, 0));
			graphic3.Points.Add(new PointF(0, 1000));
			graphic3.Points.Add(new PointF(1000, 0));
			graphic3.Points.Add(new PointF(1000, 1000));
			graphic3.Points.Add(graphic3.Points[0]);
			var roi3 = graphic3.GetRoi();
            Assert.IsInstanceOf(typeof(PolygonalRoi), roi3, "PolylineGraphic.GetRoi() should return PolygonalRoi for shape {0}", Format(graphic3.Points));
		}

		private static string Format<T>(IEnumerable<T> collection)
		{
			const string separator = ", ";

			var builder = new StringBuilder();
			foreach (var item in collection)
			{
				builder.Append(item.ToString());
				builder.Append(separator);
			}

			if (builder.Length > separator.Length)
				builder.Remove(builder.Length - separator.Length, separator.Length);

			builder.Insert(0, '{');
			builder.Insert(builder.Length, '}');
			return builder.ToString();
		}
	}
}

#endif