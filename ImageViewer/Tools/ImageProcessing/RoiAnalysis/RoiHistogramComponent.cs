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
using System.Text;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.RoiAnalysis
{
	/// <summary>
	/// Extension point for views onto <see cref="RoiHistogramComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class RoiHistogramComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// RoiHistogramComponent class
	/// </summary>
	[AssociateView(typeof(RoiHistogramComponentViewExtensionPoint))]
	public class RoiHistogramComponent : RoiAnalysisComponent
	{
		private delegate void PrepareForAnalysisDelegate(int left, int top, int right, int bottom);
		private delegate bool PointInRoiDelegate(int x, int y);

		private int _minBin = -200;
		private int _maxBin = 1000;
		private int _numBins = 100;
		private int[] _binLabels;
		private int[] _bins;
		private bool _autoscale;

		private PrepareForAnalysisDelegate _prepareForAnalysisDelegate;
		private PointInRoiDelegate _pointInRoiDelegate;

		private float _a2;
		private float _b2;
		private float _cx;
		private float _cy;

		/// <summary>
		/// Constructor
		/// </summary>
		public RoiHistogramComponent(IImageViewerToolContext imageViewerToolContext)
			: base(imageViewerToolContext)
		{
		}

		public int MinBin
		{
			get { return _minBin; }
			set 
			{ 
				_minBin = value;
				this.NotifyPropertyChanged("MinBin");
				OnAllPropertiesChanged();
			}
		}

		public int MaxBin
		{
			get { return _maxBin; }
			set 
			{ 
				_maxBin = value;
				this.NotifyPropertyChanged("MaxBin");
				OnAllPropertiesChanged();
			}
		}

		public int NumBins
		{
			get { return _numBins; }
			set 
			{ 
				_numBins = value;
				this.NotifyPropertyChanged("NumBins");
				OnAllPropertiesChanged();
			}
		}

		public bool AutoScale
		{
			get { return _autoscale; }
			set 
			{ 
				_autoscale = value;
				this.NotifyPropertyChanged("AutoScale");
				OnAllPropertiesChanged();
			}
		}

		public int[] BinLabels
		{
			get { return _binLabels; }
		}

		public int[] Bins
		{
			get { return _bins; }
		}

		public bool ComputeHistogram()
		{
			RectangleInteractiveGraphic rectangle = GetRoi<RectangleInteractiveGraphic>();
			EllipseInteractiveGraphic ellipse = GetRoi<EllipseInteractiveGraphic>();

			if (rectangle != null)
			{
				_prepareForAnalysisDelegate = null;
				_pointInRoiDelegate = PointInRectangle;
				return ComputeHistogram(rectangle);
			}
			else if (ellipse != null)
			{
				_prepareForAnalysisDelegate = PrepareEllipseForAnalysis;
				_pointInRoiDelegate = PointInEllipse;
				return ComputeHistogram(ellipse);
			}

			this.Enabled = false;
			return false;
		}

		private bool ComputeHistogram(BoundableInteractiveGraphic roi)
		{
			IImageGraphicProvider imageGraphicProvider =
				roi.ParentPresentationImage as IImageGraphicProvider;

			if (imageGraphicProvider == null)
			{
				this.Enabled = false;
				return false;
			}

			// For now, only allow ROIs of grayscale images
			GrayscaleImageGraphic image = imageGraphicProvider.ImageGraphic as GrayscaleImageGraphic;
			if (image == null)
			{
				this.Enabled = false;
				return false;
			}

			roi.CoordinateSystem = CoordinateSystem.Source;
			int left = (int)Math.Round(roi.BoundingBox.Left);
			int right = (int)Math.Round(roi.BoundingBox.Right);
			int top = (int)Math.Round(roi.BoundingBox.Top);
			int bottom = (int)Math.Round(roi.BoundingBox.Bottom);
			roi.ResetCoordinateSystem();

			// If any part of the ROI is outside the bounds of the image,
			// don't allow a histogram to be displayed since it's invalid.
			if (left < 0 || left > image.Columns - 1 ||
				right < 0 || right > image.Columns - 1 ||
				top < 0 || top > image.Rows - 1 ||
				bottom < 0 || bottom > image.Rows - 1)
			{
				this.Enabled = false;
				return false;
			}

			int width = Math.Abs(right - left) + 1;
			int height = Math.Abs(bottom - top) + 1;

			int[] roiPixelData = new int[width * height];

			if (_prepareForAnalysisDelegate != null)
				_prepareForAnalysisDelegate(left, top, right, bottom);

			IComposableLut modalityLut = image.ModalityLut;
			GrayscalePixelData pixelData = image.PixelData;

			pixelData.ForEachPixel(left, top, right, bottom,
				delegate(int i, int x, int y, int pixelIndex)
				{
					if (_pointInRoiDelegate(x, y))
					{
						roiPixelData[i] = modalityLut[pixelData.GetPixel(pixelIndex)];
					}
					else
					{
						//this won't show up on the histogram plot.
						roiPixelData[i] = int.MaxValue;
					}
				});

			Histogram histogram = new Histogram(
				_minBin, _maxBin, _numBins, roiPixelData);

			_bins = histogram.Bins;
			_binLabels = histogram.BinLabels;

			NotifyPropertyChanged("MinBin");
			NotifyPropertyChanged("MaxBin");
			NotifyPropertyChanged("NumBins");

			this.Enabled = true;
			return true;
		}

		private T GetRoi<T>() where T : BoundableInteractiveGraphic
		{
			RoiGraphic graphic = GetSelectedRoi();
			if (graphic == null)
				return null;

			return graphic.Roi as T;
		}

		private void PrepareEllipseForAnalysis(int left, int top, int right, int bottom)
		{
			float a = (right - left) / 2;
			_a2 = a * a;
			float b = (bottom - top) / 2;
			_b2 = b * b;
			_cx = left + a;
			_cy = top + b;
		}

		private bool PointInEllipse(int x, int y)
		{
			float xp = x - _cx;
			float yp = y - _cy;
			return (xp * xp / _a2 + yp * yp / _b2) <= 1;
		}

		private static bool PointInRectangle(int x, int y)
		{
			return true;
		}

		protected override bool CanAnalyzeSelectedRoi()
		{
			return GetRoi<EllipseInteractiveGraphic>() != null ||
				   GetRoi<RectangleInteractiveGraphic>() != null;
		}
	}
}
