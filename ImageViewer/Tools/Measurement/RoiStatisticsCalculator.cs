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

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	public delegate bool IsPointInRoiDelegate(int x, int y);

	public static class RoiStatisticsCalculator
	{
		public static string Calculate(RoiInfo roiInfo, IsPointInRoiDelegate isPointInRoi)
		{
			StringBuilder sb = new StringBuilder();

			bool isGrayscale = roiInfo.PixelData is GrayscalePixelData;

			if (isGrayscale && IsBoundingBoxInImage(roiInfo.BoundingBox, roiInfo.ImageColumns, roiInfo.ImageRows))
			{
				if (roiInfo.Mode == RoiAnalysisMode.Responsive)
				{
					sb.AppendFormat(SR.ToolsMeasurementFormatMean, SR.ToolsMeasurementNoValue);
					sb.AppendLine();
					sb.AppendFormat(SR.ToolsMeasurementFormatStdDev, SR.ToolsMeasurementNoValue);
					return sb.ToString();
				}

				int mean = CalculateMean(
					roiInfo.BoundingBox, 
					(GrayscalePixelData)roiInfo.PixelData, 
					roiInfo.ModalityLut, 
					isPointInRoi);
				
				int stdDev = CalculateStandardDeviation(
					mean, 
					roiInfo.BoundingBox, 
					(GrayscalePixelData)roiInfo.PixelData, 
					roiInfo.ModalityLut, 
					isPointInRoi);

				if (roiInfo.Modality == "CT")
				{
					sb.AppendFormat(SR.ToolsMeasurementFormatMeanCT, mean);
					sb.AppendLine();
					sb.AppendFormat(SR.ToolsMeasurementFormatStdDevCT, stdDev);
				}
				else
				{
					sb.AppendFormat(SR.ToolsMeasurementFormatMean, mean);
					sb.AppendLine();
					sb.AppendFormat(SR.ToolsMeasurementFormatStdDev, stdDev);
				}
			}
			else
			{
				sb.AppendFormat(SR.ToolsMeasurementFormatMean, SR.NotApplicable);
				sb.AppendLine();
				sb.AppendFormat(SR.ToolsMeasurementFormatStdDev, SR.NotApplicable);
			}

			return sb.ToString();
		}

		private static int CalculateMean
			(
				RectangleF roiBoundingBox,
				GrayscalePixelData pixelData, 
				IComposableLut modalityLut,
				IsPointInRoiDelegate isPointInRoi
			)
		{
			Platform.CheckForNullReference(pixelData, "pixelData");
			Platform.CheckForNullReference(isPointInRoi, "isPointInRoi");

			Int64 sum = 0;
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

			int mean;

			if (pixelCount == 0)
				mean = 0;
			else
				mean = (int)Math.Round(sum/(double)pixelCount);

			return mean;
		}

		private static int CalculateStandardDeviation
			(
				int mean,
				RectangleF roiBoundingBox,
				GrayscalePixelData pixelData, 
				IComposableLut modalityLut,
				IsPointInRoiDelegate isPointInRoi
			)
		{
			Platform.CheckForNullReference(pixelData, "pixelData");
			Platform.CheckForNullReference(isPointInRoi, "isPointInRoi");

			Int64 sum = 0;
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
							int deviation = pixelData.GetPixel(pixelIndex);
							if (modalityLut != null)
								deviation = modalityLut[deviation];

							deviation -= mean;
							sum += deviation * deviation;
						}
					});

			int stdDev;

			if (pixelCount == 0)
				stdDev = 0;
			else
				stdDev = (int)Math.Round(Math.Sqrt(sum / (double)pixelCount));

			return stdDev;
		}

		private static bool IsBoundingBoxInImage(RectangleF boundingBox, float imageColumns, float imageRows)
		{
			boundingBox = RectangleUtilities.ConvertToPositiveRectangle(boundingBox);

			if (boundingBox.Width == 0 || boundingBox.Height == 0)
				return false;

			if (boundingBox.Left < 0 ||
				boundingBox.Top < 0 ||
				boundingBox.Right > (imageColumns - 1) ||
				boundingBox.Bottom > (imageRows - 1))
				return false;

			return true;
		}

	}
}