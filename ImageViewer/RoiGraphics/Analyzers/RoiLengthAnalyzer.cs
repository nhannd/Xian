#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
	    private RoiAnalyzerUpdateCallback _updateCallback;

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
        //public string Analyze(Roi roi, RoiAnalysisMode mode)
        //{
        //    if (!SupportsRoi(roi))
        //        return null;

        //    IRoiLengthProvider lengthProvider = (IRoiLengthProvider) roi;

        //    string text;

        //    Units oldUnits = lengthProvider.Units;
        //    lengthProvider.Units = lengthProvider.IsCalibrated ? _units : Units.Pixels;

        //    if (!lengthProvider.IsCalibrated || _units == Units.Pixels)
        //        text = String.Format(SR.FormatLengthPixels, lengthProvider.Length);
        //    else if (_units == Units.Millimeters)
        //        text = String.Format(SR.FormatLengthMm, lengthProvider.Length);
        //    else
        //        text = String.Format(SR.FormatLengthCm, lengthProvider.Length);

        //    lengthProvider.Units = oldUnits;

        //    return text;
        //}

        public IRoiAnalyzerResult Analyze(Roi roi, RoiAnalysisMode mode)
        {
            if (!SupportsRoi(roi))
                return null;

            IRoiLengthProvider lengthProvider = (IRoiLengthProvider)roi;

		    Units oldUnits = lengthProvider.Units;
            lengthProvider.Units = lengthProvider.IsCalibrated ? _units : Units.Pixels;

		    IRoiAnalyzerResult result;

            if (!lengthProvider.IsCalibrated || _units == Units.Pixels)
            {
                //text = String.Format(SR.FormatLengthPixels, lengthProvider.Length);
                result = new SingleValueRoiAnalyzerResult("Length", SR.FormatLengthPixels, lengthProvider.Length,
                                                          String.Format(SR.FormatLengthPixels, lengthProvider.Length));

            }
            else if (_units == Units.Millimeters)
            {
                //text = String.Format(SR.FormatLengthMm, lengthProvider.Length);
                result = new SingleValueRoiAnalyzerResult("Length", SR.FormatLengthMm, lengthProvider.Length,
                                                          String.Format(SR.FormatLengthMm, lengthProvider.Length));

            }
            else
            {
                //text = String.Format(SR.FormatLengthCm, lengthProvider.Length);
                result = new SingleValueRoiAnalyzerResult("Length", SR.FormatLengthCm, lengthProvider.Length,
                                                          String.Format(SR.FormatLengthCm, lengthProvider.Length));

            }

            lengthProvider.Units = oldUnits;

            return result;
            
        }

        public void SetRoiAnalyzerUpdateCallback(RoiAnalyzerUpdateCallback callback)
        {
            _updateCallback = callback;
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