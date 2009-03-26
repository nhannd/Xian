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
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.RoiGraphics.Analyzers
{
	[ExtensionOf(typeof (RoiAnalyzerExtensionPoint))]
	public class RoiLengthAnalyzer : IRoiAnalyzer
	{
		public bool SupportsRoi(Roi roi)
		{
			return roi is IRoiLengthProvider;
		}

		public string Analyze(Roi roi, RoiAnalysisMode mode)
		{
			if (!SupportsRoi(roi))
				return null;

			Units units = Units.Centimeters;

			IRoiLengthProvider lengthProvider = (IRoiLengthProvider) roi;

			string text;

			if (!lengthProvider.IsCalibrated || units == Units.Pixels)
				text = String.Format(SR.FormatLengthPixels, lengthProvider.PixelLength);
			else if (units == Units.Millimeters)
				text = String.Format(SR.FormatLengthMm, lengthProvider.Length);
			else
				text = String.Format(SR.FormatLengthCm, lengthProvider.Length / 10);

			return text;
		}

		#region Public Static Helpers

		public static double CalculateLength(
			PointF point1,
			PointF point2,
			PixelSpacing normalizedPixelSpacing,
			ref Units units)
		{
			if (normalizedPixelSpacing.IsNull)
				units = Units.Pixels;

			double widthInPixels = point2.X - point1.X;
			double heightInPixels = point2.Y - point1.Y;

			double length;

			if (units == Units.Pixels)
			{
				length = Math.Sqrt(widthInPixels*widthInPixels + heightInPixels*heightInPixels);
			}
			else
			{
				double widthInMm = widthInPixels*normalizedPixelSpacing.Column;
				double heightInMm = heightInPixels*normalizedPixelSpacing.Row;
				double lengthInMm = Math.Sqrt(widthInMm*widthInMm + heightInMm*heightInMm);

				if (units == Units.Millimeters)
					length = lengthInMm;
				else
					length = lengthInMm/10;
			}

			return length;
		}

		public static double CalculateLength(
			IPointsGraphic polylineGraphic,
			PixelSpacing normalizedPixelSpacing,
			ref Units units)
		{
			return CalculateLength(
				polylineGraphic.Points[0],
				polylineGraphic.Points[1],
				normalizedPixelSpacing, ref units);
		}

		#endregion
	}
}