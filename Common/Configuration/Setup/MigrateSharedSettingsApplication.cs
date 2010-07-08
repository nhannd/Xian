using System;
using System.Xml;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common.Configuration.Setup
{
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    internal class MigrateSharedSettingsApplication : IApplicationRoot
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
			CommandLine commandLine = new CommandLine(args);
			
			foreach (SettingsGroupDescriptor group in SettingsGroupDescriptor.ListInstalledSettingsGroups(false))
				SettingsMigrator.MigrateSharedSettings(group, commandLine.PreviousExeConfigurationFilename);
        }

        #endregion
    }
}