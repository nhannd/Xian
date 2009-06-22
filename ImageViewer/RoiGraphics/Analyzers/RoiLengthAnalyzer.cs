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
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.ImageViewer.RoiGraphics.Analyzers
{
	/// <summary>
	/// An <see cref="IRoiAnalyzer"/> that displays the length of a <see cref="RoiGraphic"/>.
	/// </summary>
	[ExtensionOf(typeof (RoiAnalyzerExtensionPoint))]
	public class RoiLengthAnalyzer : IRoiAnalyzer
	{
		private Units _units = Units.Centimeters;

		/// <summary>
		/// Gets or sets the base unit of measurement in which analysis is performed.
		/// </summary>
		public Units Units
		{
			get { return _units; }
			set { _units = value; }
		}

		/// <summary>
		/// Checks if this analyzer class can analyze the given ROI.
		/// </summary>
		/// <param name="roi">The ROI to analyze.</param>
		/// <returns>True if this class can analyze the given ROI; False otherwise.</returns>
		public bool SupportsRoi(Roi roi)
		{
			return roi is IRoiLengthProvider;
		}

		/// <summary>
		/// Analyzes the given ROI.
		/// </summary>
		/// <param name="roi">The ROI being analyzed.</param>
		/// <param name="mode">The analysis mode.</param>
		/// <returns>A string containing the analysis results, which can be appended to the analysis
		/// callout of the associated <see cref="RoiGraphic"/>, if one exists.</returns>
		public string Analyze(Roi roi, RoiAnalysisMode mode)
		{
			if (!SupportsRoi(roi))
				return null;

			IRoiLengthProvider lengthProvider = (IRoiLengthProvider) roi;

			string text;

			Units oldUnits = lengthProvider.Units;
			lengthProvider.Units = lengthProvider.IsCalibrated ? _units : Units.Pixels;

			if (!lengthProvider.IsCalibrated || _units == Units.Pixels)
				text = String.Format(SR.FormatLengthPixels, lengthProvider.Length);
			else if (_units == Units.Millimeters)
				text = String.Format(SR.FormatLengthMm, lengthProvider.Length);
			else
				text = String.Format(SR.FormatLengthCm, lengthProvider.Length);

			lengthProvider.Units = oldUnits;

			return text;
		}

		#region Public Static Helpers

		/// <summary>
		/// Helper method to compute the physical distance between two pixels.
		/// </summary>
		/// <param name="point1">The first point.</param>
		/// <param name="point2">The second point.</param>
		/// <param name="normalizedPixelSpacing">The normalized pixel spacing of the image.</param>
		/// <param name="units">The units in which the resultant distance is given, passed by reference. If <paramref name="normalizedPixelSpacing"/> is not calibrated, then the passed variable will change to <see cref="RoiGraphics.Units.Pixels"/>.</param>
		/// <returns>The distance between the two points, in units of <paramref name="units"/>.</returns>
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

		#endregion
	}
}