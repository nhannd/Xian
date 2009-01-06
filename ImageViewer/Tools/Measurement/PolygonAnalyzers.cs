#region License

// Copyright (c) 2006-2009, ClearCanvas Inc.
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

#region softSurfer License (for polygon winding count code)

// Copyright 2001, softSurfer (www.softsurfer.com)
// This code may be freely used and modified for any purpose
// providing that this copyright notice is included with it.
// SoftSurfer makes no warranty for this code, and cannot be held
// liable for any real or imagined damage resulting from its use.
// Users of this code must verify correctness for their application.

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	[ExtensionOf(typeof (RoiAnalyzerExtensionPoint<PolygonRoiInfo>))]
	public class PolygonAreaCalculator : PolygonRoiAnalyzer
	{
		private static double AreaOfPolygon(RectangleF bounds, PolygonRoiInfo polygon)
		{
			CodeClock clock = new CodeClock();
			clock.Start();
			try
			{
				if (!polygon.IsComplex)
				{
					return Formula.AreaOfPolygon(polygon.Vertices);
				}
				else
				{
					// This algorithm is more computationally expensive and provides a weaker approximation of
					// the area of an n-polygon when compared with Formula.AreaOfPolygon...
					// However, it correctly handles self-intersecting polygons, so it will suffice for now
					// as long as we restrict excessive computation.
					int areaInPixels = 0;
					for (float r = bounds.Left; r <= bounds.Right; r++)
					{
						for (float c = bounds.Top; c <= bounds.Bottom; c++)
						{
							if (CountWindings(new PointF(r, c), polygon) != 0)
								areaInPixels++;
						}
					}
					return areaInPixels;
				}
			}
			finally
			{
				clock.Stop();
				Trace.WriteLine(string.Format("{1} Polygon area calculation took {0:f4} ms", clock.Seconds*1000, polygon.IsComplex ? "Complex" : "Simple"), "Measurement.Polygons");
			}
		}

		public override string Analyze(PolygonRoiInfo roiInfo)
		{
			Units units = Units.Centimeters;

			// performance enhancement to restrict excessive computation of polygon area.
			if (roiInfo.Mode == RoiAnalysisMode.Responsive)
			{
				if (units == Units.Pixels)
					return String.Format(SR.ToolsMeasurementFormatAreaPixels, SR.ToolsMeasurementNoValue);
				else if (units == Units.Millimeters)
					return String.Format(SR.ToolsMeasurementFormatAreaSquareMm, SR.ToolsMeasurementNoValue);
				else
					return String.Format(SR.ToolsMeasurementFormatAreaSquareCm, SR.ToolsMeasurementNoValue);
			}

			double areaInPixels = AreaOfPolygon(roiInfo.BoundingBox, roiInfo);

			PixelSpacing pixelSpacing = roiInfo.NormalizedPixelSpacing;

			string text;

			if (pixelSpacing.IsNull || units == Units.Pixels)
			{
				text = String.Format(SR.ToolsMeasurementFormatAreaPixels, areaInPixels);
			}
			else
			{
				double areaInMm = areaInPixels*pixelSpacing.Column*pixelSpacing.Row;

				if (units == Units.Millimeters)
					text = String.Format(SR.ToolsMeasurementFormatAreaSquareMm, areaInMm);
				else
					text = String.Format(SR.ToolsMeasurementFormatAreaSquareCm, areaInMm/100);
			}
			return text;
		}
	}

	[ExtensionOf(typeof (RoiAnalyzerExtensionPoint<PolygonRoiInfo>))]
	public class PolygonStatisticsCalculator : PolygonRoiAnalyzer
	{
		public override string Analyze(PolygonRoiInfo roiInfo)
		{
			return RoiStatisticsCalculator.Calculate(roiInfo, delegate(int x, int y) { return ContainsPoint(new PointF(x, y), roiInfo); });
		}
	}

	/// <summary>
	/// Base class providing methods commonly used in polygon analysis.
	/// </summary>
	public abstract class PolygonRoiAnalyzer : IRoiAnalyzer<PolygonRoiInfo>
	{
		internal PolygonRoiAnalyzer() {}

		/// <summary>
		/// Tests if a given polygon contains a given point within its most extreme boundaries.
		/// </summary>
		/// <param name="p">The test point.</param>
		/// <param name="polygon">A <see cref="PolygonRoiInfo"/> describing the polygon.</param>
		/// <returns>True if the polygon contains the given point; False otherwise.</returns>
		protected static bool ContainsPoint(PointF p, PolygonRoiInfo polygon)
		{
			if (!polygon.BoundingBox.Contains(p))
				return false;
			return CountWindings(p, polygon) != 0;
		}

		/// <summary>
		/// Counts the number of counter-clockwise windings that the <paramref name="polygon">polygon</paramref> makes around a given <paramref name="p">point</paramref>.
		/// </summary>
		/// <param name="p">The test point.</param>
		/// <param name="polygon">A <see cref="PolygonRoiInfo"/> describing the polygon.</param>
		/// <returns>The number of CCW windings.</returns>
		/// <seealso cref="http://softsurfer.com/Archive/algorithm_0103/algorithm_0103.htm"/>
		protected static int CountWindings(PointF p, PolygonRoiInfo polygon)
		{
			int wn = 0; // the winding number counter
			IList<PointF> vertices = polygon.Vertices;

			// loop through all edges of the polygon
			int i2 = vertices.Count - 1;
			for (int i = 0; i < vertices.Count; i2 = i, i++)
			{
				// edge from vertices[i] to vertices[i+1]
				if (vertices[i2].Y <= p.Y)
				{
					// start y <= p.y
					if (vertices[i].Y > p.Y) // an upward crossing
						if (IsLeft(vertices[i2], vertices[i], p) > 0) // p left of edge
							++wn; // have a valid up intersect
				}
				else
				{
					// start y > p.y (no test needed)
					if (vertices[i].Y <= p.Y) // a downward crossing
						if (IsLeft(vertices[i2], vertices[i], p) < 0) // p right of edge
							--wn; // have a valid down intersect
				}
			}
			return wn;
		}

		/// <summary>Used by <see cref="CountWindings"/>.</summary>
		/// <seealso cref="http://softsurfer.com/Archive/algorithm_0103/algorithm_0103.htm"/>
		private static int IsLeft(PointF p0, PointF p1, PointF p2)
		{
			float result = (p1.X - p0.X)*(p2.Y - p0.Y) - (p2.X - p0.X)*(p1.Y - p0.Y);
			return FloatComparer.Compare(result, 0, 1);
		}

		public abstract string Analyze(PolygonRoiInfo roiInfo);
	}
}