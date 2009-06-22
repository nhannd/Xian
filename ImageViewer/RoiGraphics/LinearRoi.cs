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

using System;
using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.RoiGraphics.Analyzers;

namespace ClearCanvas.ImageViewer.RoiGraphics
{
	/// <summary>
	/// Represents a static, linear region of interest for the purposes of computing statistics on the pixels along the length.
	/// </summary>
	public class LinearRoi : Roi, IRoiLengthProvider
	{
		private readonly IList<PointF> _points;
		private Units _units;

		/// <summary>
		/// Constructs a new linear region of interest.
		/// </summary>
		/// <param name="point1">The first end point of the line.</param>
		/// <param name="point2">The second end point of the line.</param>
		/// <param name="presentationImage">The image containing the source pixel data.</param>
		public LinearRoi(PointF point1, PointF point2, IPresentationImage presentationImage) : base(presentationImage)
		{
			List<PointF> points = new List<PointF>();
			points.Add(point1);
			points.Add(point2);
			_points = points.AsReadOnly();
		}


		/// <summary>
		/// Constructs a new linear region of interest.
		/// </summary>
		/// <param name="points">The end points of the line.</param>
		/// <param name="presentationImage">The image containing the source pixel data.</param>
		public LinearRoi(IEnumerable<PointF> points, IPresentationImage presentationImage) : base(presentationImage)
		{
			_points = new List<PointF>(points).AsReadOnly();

			Platform.CheckTrue(_points.Count >= 2, "At least 2 points must be specified.");
		}

		/// <summary>
		/// Constructs a new linear region of interest, specifying an <see cref="IPointsGraphic"/> as the source of the definition and pixel data.
		/// </summary>
		/// <param name="polyline">The linear graphic that represents the region of interest.</param>
		public LinearRoi(IPointsGraphic polyline) : base(polyline.ParentPresentationImage)
		{
			polyline.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				List<PointF> points = new List<PointF>();
				foreach(PointF point in polyline.Points)
				{
					points.Add(point);
				}
				_points = points.AsReadOnly();
			}
			finally
			{
				polyline.ResetCoordinateSystem();
			}

			Platform.CheckTrue(_points.Count >= 2, "At least 2 points must be specified.");
		}

		/// <summary>
		/// Gets the points that define the linear region of interest.
		/// </summary>
		public IList<PointF> Points
		{
			get { return _points; }
		}

		/// <summary>
		/// Gets or sets the units of area with which to compute the value of <see cref="IRoiAreaProvider.Area"/>.
		/// </summary>
		public Units Units
		{
			get { return _units; }
			set { _units = value; }
		}

		/// <summary>
		/// Gets a value indicating that the image has pixel spacing information or has
		/// previously been calibrated with physical dimensions.
		/// </summary>
		public bool IsCalibrated
		{
			get { return !base.NormalizedPixelSpacing.IsNull; }
		}

		/// <summary>
		/// Called by <see cref="Roi.BoundingBox"/> to compute the tightest bounding box of the region of interest.
		/// </summary>
		/// <remarks>
		/// <para>This method is only called once and the result is cached for future accesses.</para>
		/// <para>
		/// Regions of interest have no notion of coordinate system. All coordinates are inherently
		/// given relative to the image pixel space (i.e. <see cref="CoordinateSystem.Source"/>.)
		/// </para>
		/// </remarks>
		/// <returns>A rectangle defining the bounding box.</returns>
		protected override RectangleF ComputeBounds()
		{
			return RectangleUtilities.ComputeBoundingRectangle(_points);
		}

		/// <summary>
		/// Creates a copy of this <see cref="Roi"/> using the same region of interest shape but using a different image as the source pixel data.
		/// </summary>
		/// <param name="presentationImage">The image upon which to copy this region of interest.</param>
		/// <returns>A new <see cref="Roi"/> of the same type and shape as the current region of interest.</returns>
		public override Roi CopyTo(IPresentationImage presentationImage)
		{
			return new LinearRoi(_points, presentationImage);
		}

		/// <summary>
		/// Tests to see if the given point is contained within the region of interest.
		/// </summary>
		/// <remarks>
		/// Regions of interest have no notion of coordinate system. All coordinates are inherently
		/// given relative to the image pixel space (i.e. <see cref="CoordinateSystem.Source"/>.)
		/// </remarks>
		/// <param name="point">The point to test.</param>
		/// <returns>True if the point is defined as within the region of interest; False otherwise.</returns>
		public override bool Contains(PointF point)
		{
			PointF topLeft = new PointF((float) Math.Floor(point.X), (float) Math.Floor(point.Y));
			for (int n = 1; n < _points.Count; n++)
			{
				PointF intersection;
				if (Vector.LineSegmentIntersection(topLeft, topLeft + new SizeF(1, 1), _points[n - 1], _points[n], out intersection) != Vector.LineSegments.DoNotIntersect)
					return true;
			}
			return false;
		}

		#region IRoiLengthProvider Members

		private double? _lengthInPixels;
		private double? _lengthInMm;

		/// <summary>
		/// Gets the length of the region of interest in units of length as specified by <see cref="IRoiLengthProvider.Units"/>.
		/// </summary>
		/// <exception cref="InvalidOperationException">If <see cref="IRoiLengthProvider.Units"/> is a physical
		/// unit of measurement and the image has no pixel spacing information, nor has it been calibrated.</exception>
		public double Length
		{
			get
			{
				if (!ValidateUnits(_units, base.NormalizedPixelSpacing))
					throw new InvalidOperationException("Pixel spacing must be calibrated in order to compute physical units.");

				if (_units == Units.Pixels)
				{
					if (!_lengthInPixels.HasValue)
					{
						double length = 0;
						for (int n = 1; n < _points.Count; n++)
						{
							length += Vector.Distance(_points[n - 1], _points[n]);
						}
						_lengthInPixels = length;
					}
					return _lengthInPixels.Value;
				}
				else
				{
					if (!_lengthInMm.HasValue)
					{
						Units units = Units.Millimeters;
						double length = 0;
						for (int n = 1; n < _points.Count; n++)
						{
							length += RoiLengthAnalyzer.CalculateLength(_points[n - 1], _points[n], base.NormalizedPixelSpacing, ref units);
						}
						_lengthInMm = length;
					}

					if (_units == Units.Centimeters)
						return _lengthInMm.Value/10;
					return _lengthInMm.Value;
				}
			}
		}

		#endregion
	}
}