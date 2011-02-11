#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Services.Tools
{
	[ExtensionOf(typeof(ApplicationRootExtensionPoint))]
	public class ReindexLocalDataStoreApplication : IApplicationRoot
	{
		private class CommandLine : ClearCanvas.Common.Utilities.CommandLine
		{
			public CommandLine()
			{
				TimeoutSeconds = 10;
				Silent = NoWait = false;
			}

			[CommandLineParameter("silent", "s", "Specifies whether the application should be silent (e.g. console app).")]
			public bool Silent { get; set; }

			[CommandLineParameter("nowait", "n", "When true, the application will initiate the reindex and quit without waiting for it to complete.  Only works with 'silent' switch.")]
			public bool NoWait { get; set; }

			[CommandLineParameter("timeout", "t", "The amount of time, in seconds, to wait before reindex activity is detected before quitting.")]
			public int TimeoutSeconds { get; set; }
		}

		#region IApplicationRoot Members

		public void RunApplication(string[] args)
		{
			var commandLine = new CommandLine();
			commandLine.Parse(args);

			if (!commandLine.Silent)
				((IApplicationRoot)new ReindexLocalDataStoreDesktopApplication{TimeoutSeconds = commandLine.TimeoutSeconds}).RunApplication(new string[0]);
			else
				new ReindexLocalDataStoreConsoleApplication{NoWait = commandLine.NoWait, TimeoutSeconds = commandLine.TimeoutSeconds}.Run();
		}

		#endregion
	}
}
