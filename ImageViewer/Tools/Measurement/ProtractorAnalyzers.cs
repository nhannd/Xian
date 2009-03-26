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
using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.RoiGraphics;
using ClearCanvas.ImageViewer.RoiGraphics.Analyzers;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	[ExtensionOf(typeof(RoiAnalyzerExtensionPoint))]
	public class ProtractorAngleCalculator : IRoiAnalyzer
	{
		public string Analyze(ProtractorRoiInfo roiInfo, RoiAnalysisMode mode)
		{
			// Don't show the callout until the second ray is drawn
			if (roiInfo.Points.Count < 3)
				return SR.ToolsMeasurementSetVertex;

			List<PointF> normalizedPoints = NormalizePoints(roiInfo);

			double angle = Vector.SubtendedAngle(normalizedPoints[0], normalizedPoints[1], normalizedPoints[2]);

			return String.Format(SR.ToolsMeasurementFormatDegrees, Math.Abs(angle));
		}

		private List<PointF> NormalizePoints(ProtractorRoiInfo roiInfo)
		{
			float aspectRatio = 1F;

			if (roiInfo.PixelAspectRatio.IsNull)
			{
				if (!roiInfo.NormalizedPixelSpacing.IsNull)
					aspectRatio = (float)(roiInfo.NormalizedPixelSpacing.Row/roiInfo.NormalizedPixelSpacing.Column);
			}
			else
			{
				aspectRatio = (float)(roiInfo.PixelAspectRatio.Row/roiInfo.PixelAspectRatio.Column);
			}

			List<PointF> normalized = new List<PointF>();
			foreach (PointF point in roiInfo.Points)
				normalized.Add(new PointF(point.X, point.Y * aspectRatio));

			return normalized;
		}

		public string Analyze(Roi roi, RoiAnalysisMode mode)
		{
			return Analyze((ProtractorRoiInfo) roi, mode);
		}

		public bool SupportsRoi(Roi roi)
		{
			return roi is ProtractorRoiInfo;
		}
	}
}
