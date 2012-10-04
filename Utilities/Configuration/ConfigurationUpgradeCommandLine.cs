#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Utilities.Configuration
{
	/// <summary>
	/// Command line attributes for <see cref="ConfigurationUpgradeApplication"/>.
	/// </summary>
	public class ConfigurationUpgradeCommandLine : CommandLine
	{
		/// <summary>
		/// Specifies the new release's configuration file.
		/// </summary>
		[CommandLineParameter(0, "Specifies the path to the new release configuration file.", Required = true)]
		public string NewConfigFile
		{ get; set; }

		/// <summary>
		/// Specifies the current configuration file.
		/// </summary>
		[CommandLineParameter(1, "Specifies the path to the source configuration file to upgrade.", Required = true)]
		public string OldConfigFile
		{ get; set; }

		/// <summary>
		/// Specifies the output (upgraded) configuration file.
		/// </summary>
		[CommandLineParameter(2, "Specifies the path to the output/upgraded configuration file.", Required = true)]
		public string OutputConfigFile
		{ get; set; }

		/// <summary>
		/// Specifies the output configuration file.
		/// </summary>
		[CommandLineParameter("x", "Specifies the name of an exceptions file.", Required = false)]
		public string ExceptionsFile
		{ get; set; }

		/// <summary>
		/// Specifies the output configuration file.
		/// </summary>
		[CommandLineParameter("d", "Specifies whether or not to add debug comments.", Required = false)]
		public bool OutputDebugComments
		{ get; set; }
	}
}