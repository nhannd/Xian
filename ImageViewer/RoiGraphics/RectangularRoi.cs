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
using System.Drawing;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.RoiGraphics
{
	public class RectangularRoi : Roi, IRoiAreaProvider, IRoiStatisticsProvider
	{
		private readonly RectangleF _rectangle;

		public RectangularRoi(IBoundableGraphic rectangle) : base(rectangle.ParentPresentationImage)
		{
			rectangle.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				_rectangle = rectangle.BoundingBox;
			}
			finally
			{
				rectangle.ResetCoordinateSystem();
			}
		}

		public RectangularRoi(RectangleF rectangle, IPresentationImage presentationImage) : base(presentationImage)
		{
			_rectangle = rectangle;
		}

		public RectangleF Rectangle
		{
			get { return _rectangle; }
		}

		protected override RectangleF ComputeBounds()
		{
			return _rectangle;
		}

		public override Roi CopyTo(IPresentationImage presentationImage)
		{
			return new RectangularRoi(_rectangle, presentationImage);
		}

		public override bool Contains(PointF point)
		{
			return _rectangle.Contains(point);
		}

		/// <summary>
		/// Gets a value indicating that the image has pixel spacing information
		/// or has previously been calibrated, and hence the <see cref="Area"/>
		/// and <see cref="Perimeter"/> properties are available.
		/// </summary>
		public bool IsCalibrated {
			get { return !base.NormalizedPixelSpacing.IsNull; }
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
					_pixelArea = Math.Abs(_rectangle.Width*_rectangle.Height);
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

		#endregion
	}
}