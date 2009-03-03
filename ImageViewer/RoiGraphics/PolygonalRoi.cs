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

using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.RoiGraphics
{
	public class PolygonalRoi : Roi, IRoiStatisticsProvider, IRoiAreaProvider
	{
		private readonly PolygonF _polygon;

		public PolygonalRoi(IList<PointF> vertices, IPresentationImage presentationImage) : base(presentationImage)
		{
			_polygon = new PolygonF(vertices);
		}

		public PolygonalRoi(PolygonInteractiveGraphic polygon)
			: base(polygon.ParentPresentationImage)
		{
			polygon.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				if (polygon.VertexCount >= 3)
				{
					List<PointF> vertices = new List<PointF>(polygon.VertexCount);
					for (int n = 0; n < polygon.VertexCount; n++)
					{
						vertices.Add(polygon.PolyLine[n]);
					}
					_polygon = new PolygonF(vertices);
				}
				else
				{
					_polygon = null;
				}
			}
			finally
			{
				polygon.ResetCoordinateSystem();
			}
		}

		/// <summary>
		/// Gets an object representing the polygon.
		/// </summary>
		public PolygonF Polygon
		{
			get { return _polygon; }
		}

		protected override RectangleF ComputeBounds()
		{
			if (_polygon == null)
				return RectangleF.Empty;
			return _polygon.BoundingRectangle;
		}

		public override Roi CopyTo(IPresentationImage presentationImage)
		{
			return new PolygonalRoi(_polygon.Vertices, presentationImage);
		}

		public override bool Contains(PointF point)
		{
			return _polygon != null && _polygon.Contains(point);
		}

		#region IRoiStatisticsProvider Members

		private RoiStatistics _statistics;

		public double StandardDeviation
		{
			get
			{
				if (_statistics == null)
				{
					_statistics = RoiStatistics.Calculate(this);
				}
				return _statistics.StandardDeviation;
			}
		}

		public double Mean
		{
			get
			{
				if (_statistics == null)
				{
					_statistics = RoiStatistics.Calculate(this);
				}
				return _statistics.Mean;
			}
		}

		#endregion

		#region IRoiAreaProvider Members

		private double? _pixelArea;
		private double? _area;

		public double PixelArea
		{
			get
			{
				if (!_pixelArea.HasValue)
				{
					if (_polygon == null)
						_pixelArea = 0;
					else
						_pixelArea = _polygon.ComputeArea();
				}
				return _pixelArea.Value;
			}
		}

		public double Area
		{
			get
			{
				if (!_area.HasValue)
				{
					if (base.NormalizedPixelSpacing.IsNull)
						throw new UncalibratedImageException();

					_area = this.PixelArea*base.NormalizedPixelSpacing.Column*base.NormalizedPixelSpacing.Row;
				}
				return _area.Value;
			}
		}

		public bool IsCalibrated
		{
			get { return !base.NormalizedPixelSpacing.IsNull; }
		}

		#endregion
	}
}