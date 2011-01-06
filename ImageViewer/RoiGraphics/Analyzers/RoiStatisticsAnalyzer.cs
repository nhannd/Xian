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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.RoiGraphics.Analyzers
{
	/// <summary>
	/// An <see cref="IRoiAnalyzer"/> that displays common statistics of a <see cref="RoiGraphic"/>.
	/// </summary>
	[ExtensionOf(typeof (RoiAnalyzerExtensionPoint))]
	public class RoiStatisticsAnalyzer : IRoiAnalyzer
	{
		/// <summary>
		/// This property is not applicable to this analyzer.
		/// </summary>
		Units IRoiAnalyzer.Units
		{
			get { return Units.Centimeters; }
			set { }
		}

		/// <summary>
		/// Checks if this analyzer class can analyze the given ROI.
		/// </summary>
		/// <remarks>
		/// Implementations should return a result based on the type of ROI, not on the particular current state of the ROI.
		/// </remarks>
		/// <param name="roi">The ROI to analyze.</param>
		/// <returns>True if this class can analyze the given ROI; False otherwise.</returns>
		public bool SupportsRoi(Roi roi)
		{
			return roi is IRoiStatisticsProvider;
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

			IRoiStatisticsProvider statisticsProvider = (IRoiStatisticsProvider) roi;

			StringBuilder sb = new StringBuilder();

			bool isGrayscale = roi.PixelData is GrayscalePixelData;

			if (isGrayscale && roi.ContainsPixelData)
			{
				if (mode == RoiAnalysisMode.Responsive)
				{
					sb.AppendFormat(SR.FormatMean, SR.StringNoValue);
					sb.AppendLine();
					sb.AppendFormat(SR.FormatStdDev, SR.StringNoValue);
					return sb.ToString();
				}

				double mean = statisticsProvider.Mean;
				double stdDev = statisticsProvider.StandardDeviation;

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
	}
}