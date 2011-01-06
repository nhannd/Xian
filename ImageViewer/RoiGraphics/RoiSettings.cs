#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Configuration;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.RoiGraphics
{
	/// <summary>
	/// A helper class for reading ROI settings.
	/// </summary>
	public static class RoiSettingsHelper
	{
		/// <summary>
		/// Gets a value indicating whether or not ROI stats should be shown on new ROI objects by default.
		/// </summary>
		public static bool ShowAnalysisByDefault
		{
			get { return RoiSettings.Default.ShowAnalysisByDefault; }
		}

		/// <summary>
		/// Gets a value indicating the preferred linear, area and volume units of ROI analysis output.
		/// </summary>
		public static Units AnalysisUnits
		{
			get { return RoiSettings.Default.AnalysisUnits; }
		}
	}

	[SettingsGroupDescription("Configures ROI settings.")]
	[SettingsProvider(typeof (StandardSettingsProvider))]
	internal sealed partial class RoiSettings
	{
		public RoiSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}
	}
}