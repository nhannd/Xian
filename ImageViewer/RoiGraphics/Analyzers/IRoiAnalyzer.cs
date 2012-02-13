#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.RoiGraphics.Analyzers
{
	/// <summary>
	/// Extension point for <see cref="IRoiAnalyzer"/>s that are automatically discovered and invoked by the <see cref="RoiCalloutGraphic"/>.
	/// </summary>
	public sealed class RoiAnalyzerExtensionPoint : ExtensionPoint<IRoiAnalyzer>
	{
		/// <summary>
		/// Creates new instances of all available <see cref="IRoiAnalyzer"/>s extending <see cref="RoiAnalyzerExtensionPoint"/>.
		/// </summary>
		/// <returns>An enumeration of <see cref="IRoiAnalyzer"/>s.</returns>
		public static IEnumerable<IRoiAnalyzer> CreateRoiAnalyzers()
		{
			SortedList<string, IRoiAnalyzer> extensions = new SortedList<string, IRoiAnalyzer>();
			foreach (IRoiAnalyzer roiAnalyzer in new RoiAnalyzerExtensionPoint().CreateExtensions())
			{
				roiAnalyzer.Units = RoiSettings.Default.AnalysisUnits;
				extensions.Add(roiAnalyzer.GetType().FullName, roiAnalyzer);
			}
			return extensions.Values;
		}
	}

	/// <summary>
	/// Enumerated values for the type of ROI analysis to perform.
	/// </summary>
	public enum RoiAnalysisMode
	{
		/// <summary>
		/// Indicates that normal analysis should be performed.
		/// </summary>
		Normal = 0,

		/// <summary>
		/// Indicates that the analysis is being performed in response to live changes, and that only fast analysis should be performed.
		/// </summary>
		Responsive = 1
	}


    public delegate void RoiAnalyzerUpdateCallback();

	/// <summary>
	/// Interface for all ROI analyzers.
	/// </summary>
	public interface IRoiAnalyzer
	{
	    
		/// <summary>
		/// Gets or sets the base unit of measurement in which analysis is performed.
		/// </summary>
		Units Units { get; set; }

		/// <summary>
		/// Checks if this analyzer class can analyze the given ROI.
		/// </summary>
		/// <remarks>
		/// Implementations should return a result based on the type of ROI, not on the particular current state of the ROI.
		/// </remarks>
		/// <param name="roi">The ROI to analyze.</param>
		/// <returns>True if this class can analyze the given ROI; False otherwise.</returns>
		bool SupportsRoi(Roi roi);

		/// <summary>
		/// Analyzes the given ROI.
		/// </summary>
		/// <param name="roi">The ROI being analyzed.</param>
		/// <param name="mode">The analysis mode.</param>
		/// <returns>A string containing the analysis results, which can be appended to the analysis
		/// callout of the associated <see cref="RoiGraphic"/>, if one exists.</returns>
		string Analyze(Roi roi, RoiAnalysisMode mode);

	    void SetRoiAnalyzerUpdateCallback(RoiAnalyzerUpdateCallback callback);
	}
}