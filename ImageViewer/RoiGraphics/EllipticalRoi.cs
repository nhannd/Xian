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
	public class EllipticalRoi : Roi, IRoiAreaProvider, IRoiStatisticsProvider
	{
		private readonly RectangleF _bounds;
		private readonly float a, b, a2, b2, h, k;
		private Units _units;

		public EllipticalRoi(RectangleF boundingBox, IPresentationImage presentationImage)
			: base(presentationImage)
		{
			_bounds = boundingBox;

			a = _bounds.Width/2;
			b = _bounds.Height/2;
			a2 = a*a;
			b2 = b*b;
			h = _bounds.Left + a;
			k = _bounds.Top + b;
		}

		public EllipticalRoi(IBoundableGraphic ellipse)
			: base(ellipse.ParentPresentationImage)
		{
			ellipse.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				_bounds = ellipse.BoundingBox;

				a = _bounds.Width/2;
				b = _bounds.Height/2;
				a2 = a*a;
				b2 = b*b;
				h = _bounds.Left + a;
				k = _bounds.Top + b;
			}
			finally
			{
				ellipse.ResetCoordinateSystem();
			}
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

		protected override RectangleF ComputeBounds()
		{
			return _bounds;
		}

		public override Roi CopyTo(IPresentationImage presentationImage)
		{
			return new EllipticalRoi(_bounds, presentationImage);
		}

		public override bool Contains(PointF point)
		{
			float xh = point.X - h;
			float yk = point.Y - k;
			float r = (xh*xh/a2) + (yk*yk/b2);

			if (r <= 1)
				return true;
			else
				return false;
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

		private double? _area;

		/// <summary>
		/// Gets the area of the region of interest in the units of area as specified by <see cref="IRoiAreaProvider.Units"/>.
		/// </summary>
		/// <exception cref="UncalibratedImageException">If <see cref="IRoiAreaProvider.Units"/> is a physical
		/// unit of measurement and the image has no pixel spacing information, nor has it been calibrated.</exception>
		public double Area
		{
			get
			{
				if (!_area.HasValue)
				{
					_area = Math.Abs(_bounds.Width*_bounds.Height*Math.PI/4);
				}
				return ConvertFromSquarePixels(_area.Value, _units, base.NormalizedPixelSpacing);
			}
		}

		#endregion
	}
}