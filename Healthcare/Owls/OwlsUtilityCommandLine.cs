#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Owls
{
	/// <summary>
	/// Command line for owls utility application.
	/// </summary>
	internal class OwlsUtilityCommandLine : CommandLine
	{
		public OwlsUtilityCommandLine()
		{
			this.BatchSize = 100;
		}

		[CommandLineParameter("do", "Specifies what the utility should do (build, shrink, reindex)", Required = true)]
		public UtilityAction Action { get; set; }

		[CommandLineParameter("batch", "b", "Specifies the batch size, default is 100.")]
		public int BatchSize { get; set; }

		[CommandLineParameter("view", "v", "Specifies the name of the view to process. If not specified, all views will be processed.")]
		public string View { get; set; }

		[CommandLineParameter("connection", "c", "Specifies a connection string with elevated privileges to drop/create the view tables. Must refer to the same database as the hibernate configuration.")]
		public string ElevatedConnectionString { get; set; }
	}
}
