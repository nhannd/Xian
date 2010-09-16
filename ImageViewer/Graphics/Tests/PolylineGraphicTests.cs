#region License

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
			Assert.IsInstanceOfType(typeof (LinearRoi), roi, "PolylineGraphic.GetRoi() should return LinearRoi for shape {0}", Format(graphic.Points));
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
			Assert.IsInstanceOfType(typeof (PolygonalRoi), roi, "PolylineGraphic.GetRoi() should return PolygonalRoi for shape {0}", Format(graphic.Points));

			var graphic2 = new PolylineGraphic();
			graphic2.Points.Add(new PointF(0, 0));
			graphic2.Points.Add(new PointF(0, 1));
			graphic2.Points.Add(new PointF(1, 1));
			graphic2.Points.Add(new PointF(1, 0));
			graphic2.Points.Add(graphic2.Points[0]);
			var roi2 = graphic2.GetRoi();
			Assert.IsInstanceOfType(typeof (PolygonalRoi), roi2, "PolylineGraphic.GetRoi() should return PolygonalRoi for shape {0}", Format(graphic2.Points));

			var graphic3 = new PolylineGraphic();
			graphic3.Points.Add(new PointF(0, 0));
			graphic3.Points.Add(new PointF(0, 1000));
			graphic3.Points.Add(new PointF(1000, 0));
			graphic3.Points.Add(new PointF(1000, 1000));
			graphic3.Points.Add(graphic3.Points[0]);
			var roi3 = graphic3.GetRoi();
			Assert.IsInstanceOfType(typeof (PolygonalRoi), roi3, "PolylineGraphic.GetRoi() should return PolygonalRoi for shape {0}", Format(graphic3.Points));
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