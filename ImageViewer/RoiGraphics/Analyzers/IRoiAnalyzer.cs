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

using System.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.RoiGraphics.Analyzers
{
	/// <summary>
	/// Extension point for <see cref="IRoiAnalyzer"/>s that are automatically discovered and invoked by the <see cref="RoiCalloutGraphic"/>.
	/// </summary>
	public sealed class RoiAnalyzerExtensionPoint : ExtensionPoint<IRoiAnalyzer>
	{
		private static IList<IRoiAnalyzer> _roiAnalyzers = null;

		/// <summary>
		/// Gets an enumeration of available <see cref="IRoiAnalyzer"/>s.
		/// </summary>
		public static IEnumerable<IRoiAnalyzer> RoiAnalyzers
		{
			get
			{
				if (_roiAnalyzers == null)
				{
					SortedList<string, IRoiAnalyzer> extensions = new SortedList<string, IRoiAnalyzer>();
					foreach (IRoiAnalyzer roiAnalyzer in new RoiAnalyzerExtensionPoint().CreateExtensions())
					{
						extensions.Add(roiAnalyzer.GetType().FullName, roiAnalyzer);
					}
					_roiAnalyzers = new List<IRoiAnalyzer>(extensions.Values).AsReadOnly();
				}
				return _roiAnalyzers;
			}
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

	/// <summary>
	/// Interface for all ROI analyzers.
	/// </summary>
	public interface IRoiAnalyzer
	{
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
	}
}