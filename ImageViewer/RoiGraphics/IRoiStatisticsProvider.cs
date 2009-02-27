#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Text;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.RoiGraphics;

namespace ClearCanvas.ImageViewer.RoiGraphics
{
	/// <summary>
	/// Common interface for regions of interest that have the notion of a
	/// number of points over which the mean and standard deviation can
	/// be computed.
	/// </summary>
	public interface IRoiStatisticsProvider {
		/// <summary>
		/// Gets the standard deviation of the values over the <see cref="Roi"/>.
		/// </summary>
		double StandardDeviation { get; }

		/// <summary>
		/// Gets the mean of the values over the <see cref="Roi"/>.
		/// </summary>
		double Mean { get; }
	}

	internal class RoiStatistics
	{
		public readonly double Mean;
		public readonly double StandardDeviation;
		public readonly bool Valid;

		private RoiStatistics()
		{
			this.Valid = false;
		}

		private RoiStatistics(double mean, double stddev)
		{
			this.Valid = true;
			this.Mean = mean;
			this.StandardDeviation = stddev;
		}

		private delegate bool IsPointInRoiDelegate(int x, int y);

		public static RoiStatistics Calculate(Roi roi)
		{
			if (!(roi.PixelData is GrayscalePixelData))
				return new RoiStatistics();

			double mean = CalculateMean(
				roi.BoundingBox, 
				(GrayscalePixelData)roi.PixelData, 
				roi.ModalityLut, 
				roi.Contains);

			double stdDev = CalculateStandardDeviation(
				mean, 
				roi.BoundingBox, 
				(GrayscalePixelData)roi.PixelData, 
				roi.ModalityLut, 
				roi.Contains);

			return new RoiStatistics(mean, stdDev);
		}

		private static double CalculateMean
			(
			RectangleF roiBoundingBox,
			GrayscalePixelData pixelData, 
			IComposableLut modalityLut,
			IsPointInRoiDelegate isPointInRoi
			)
		{
			long sum = 0;
			int pixelCount = 0;

			pixelData.ForEachPixel(
				(int)roiBoundingBox.Left,
				(int)roiBoundingBox.Top,
				(int)roiBoundingBox.Right,
				(int)roiBoundingBox.Bottom,
				delegate(int i, int x, int y, int pixelIndex)
					{
						if (isPointInRoi(x, y))
						{
							++pixelCount;
							// Make sure we run the raw pixel through the modality LUT
							// when doing the calculation. Note that the modality LUT
							// can be something other than a rescale intercept, so we can't
							// just run the mean through the LUT.
							int value = pixelData.GetPixel(pixelIndex);
							if (modalityLut != null)
								value = modalityLut[value];
							
							sum += value;
						}
					});

			if (pixelCount == 0)
				return 0;

			return (double) sum/pixelCount;
		}

		private static double CalculateStandardDeviation
			(
			double mean,
			RectangleF roiBoundingBox,
			GrayscalePixelData pixelData, 
			IComposableLut modalityLut,
			IsPointInRoiDelegate isPointInRoi
			)
		{
			double sum = 0;
			int pixelCount = 0;

			pixelData.ForEachPixel(
				(int)roiBoundingBox.Left,
				(int)roiBoundingBox.Top,
				(int)roiBoundingBox.Right,
				(int)roiBoundingBox.Bottom,
				delegate(int i, int x, int y, int pixelIndex) {
					if (isPointInRoi(x, y)) {
						++pixelCount;
						int value = pixelData.GetPixel(pixelIndex);
						if (modalityLut != null)
							value = modalityLut[value];

						double deviation = value - mean;
						sum += deviation*deviation;
					}
				});

			if (pixelCount == 0)
				return 0;

			return Math.Sqrt(sum/pixelCount);
		}
	}
}