#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Xml;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common.Configuration.Setup
{
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    internal class MigrateLocalSharedSettingsApplication : IApplicationRoot
    {
		private class CommandLine : Utilities.CommandLine
		{
			public CommandLine(string[] args)
				: base(args)
			{
			}

			[CommandLineParameter(0, "The name of the local file where previous application scoped and default user settings should be taken from.", Required = true)]
			public string PreviousExeConfigurationFilename { get; set; }
		}
		
		#region IApplicationRoot Members

        public void RunApplication(string[] args)
        {
			var commandLine = new CommandLine(args);

        	var groups = SettingsGroupDescriptor.ListInstalledLocalSettingsGroups();
			foreach (var group in groups)
				SettingsMigrator.MigrateSharedSettings(group, commandLine.PreviousExeConfigurationFilename);
        }

        #endregion
    }
}