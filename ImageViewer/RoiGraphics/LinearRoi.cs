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
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.RoiGraphics.Analyzers;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.RoiGraphics
{
	public class LinearRoi : Roi, IRoiLengthProvider
	{
		private readonly IList<PointF> _points;

		public LinearRoi(PointF point1, PointF point2, IPresentationImage presentationImage) : base(presentationImage)
		{
			List<PointF> points = new List<PointF>();
			points.Add(point1);
			points.Add(point2);
			_points = points.AsReadOnly();
		}

		public LinearRoi(IEnumerable<PointF> points, IPresentationImage presentationImage) : base(presentationImage)
		{
			_points = new List<PointF>(points).AsReadOnly();

			Platform.CheckTrue(_points.Count >= 2, "At least 2 points must be specified.");
		}

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

		public IList<PointF> Points
		{
			get { return _points; }
		}

		protected override RectangleF ComputeBounds()
		{
			return RectangleUtilities.ComputeBoundingRectangle(_points);
		}

		public override Roi CopyTo(IPresentationImage presentationImage)
		{
			return new LinearRoi(_points, presentationImage);
		}

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
		private double? _length;

		public double PixelLength
		{
			get
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
		}

		public double Length
		{
			get
			{
				if (!_length.HasValue)
				{
					if (base.NormalizedPixelSpacing.IsNull)
						throw new UncalibratedImageException();

					Units units = Units.Millimeters;
					double length = 0;
					for (int n = 1; n < _points.Count; n++)
					{
						length += RoiLengthAnalyzer.CalculateLength(_points[n - 1], _points[n], base.NormalizedPixelSpacing, ref units);
					}
					_length = length;
				}
				return _length.Value;
			}
		}

		public bool IsCalibrated
		{
			get { return !base.NormalizedPixelSpacing.IsNull; }
		}

		#endregion
	}
}