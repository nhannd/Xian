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

using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	[ExtensionOf(typeof (RoiAnalyzerExtensionPoint<PolygonRoiInfo>))]
	public class PolygonAreaCalculator : IRoiAnalyzer<PolygonRoiInfo>
	{
		public string Analyze(PolygonRoiInfo roiInfo)
		{
			Units units = Units.Centimeters;

			// performance enhancement to restrict excessive computation of polygon area.
			if (roiInfo.Mode == RoiAnalysisMode.Responsive || roiInfo.Polygon == null)
			{
				if (units == Units.Pixels)
					return String.Format(SR.ToolsMeasurementFormatAreaPixels, SR.ToolsMeasurementNoValue);
				else if (units == Units.Millimeters)
					return String.Format(SR.ToolsMeasurementFormatAreaSquareMm, SR.ToolsMeasurementNoValue);
				else
					return String.Format(SR.ToolsMeasurementFormatAreaSquareCm, SR.ToolsMeasurementNoValue);
			}

			double areaInPixels = roiInfo.Polygon.ComputeArea();

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
	public class PolygonStatisticsCalculator : IRoiAnalyzer<PolygonRoiInfo>
	{
		public string Analyze(PolygonRoiInfo roiInfo)
		{
			if (roiInfo.Polygon == null)
				return "";

			return RoiStatisticsCalculator.Calculate(roiInfo, delegate(int x, int y) { return roiInfo.Polygon.Contains(new PointF(x, y)); });
		}
	}
}