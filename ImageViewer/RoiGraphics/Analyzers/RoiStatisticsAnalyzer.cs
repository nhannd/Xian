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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.RoiGraphics.Analyzers
{
	[ExtensionOf(typeof (RoiAnalyzerExtensionPoint))]
	public class RoiStatisticsAnalyzer : IRoiAnalyzer
	{
		public bool SupportsRoi(Roi roi)
		{
			return roi is IRoiStatisticsProvider;
		}

		public string Analyze(Roi roi, RoiAnalysisMode mode)
		{
			if (!SupportsRoi(roi))
				return null;

			IRoiStatisticsProvider statisticsProvider = (IRoiStatisticsProvider) roi;

			StringBuilder sb = new StringBuilder();

			bool isGrayscale = roi.PixelData is GrayscalePixelData;

			if (isGrayscale && IsBoundingBoxInImage(roi.BoundingBox, roi.ImageColumns, roi.ImageRows))
			{
				if (mode == RoiAnalysisMode.Responsive)
				{
					sb.AppendFormat(SR.FormatMean, SR.StringNoValue);
					sb.AppendLine();
					sb.AppendFormat(SR.FormatStdDev, SR.StringNoValue);
					return sb.ToString();
				}

				int mean = (int) Math.Round(statisticsProvider.Mean);

				int stdDev = (int) Math.Round(statisticsProvider.StandardDeviation);

				if (roi.Modality == "CT")
				{
					sb.AppendFormat(SR.FormatMeanCT, mean);
					sb.AppendLine();
					sb.AppendFormat(SR.FormatStdDevCT, stdDev);
				}
				else
				{
					sb.AppendFormat(SR.FormatMean, mean);
					sb.AppendLine();
					sb.AppendFormat(SR.FormatStdDev, stdDev);
				}
			}
			else
			{
				sb.AppendFormat(SR.FormatMean, SR.StringNotApplicable);
				sb.AppendLine();
				sb.AppendFormat(SR.FormatStdDev, SR.StringNotApplicable);
			}

			return sb.ToString();
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