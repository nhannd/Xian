using System;
using ClearCanvas.Common.Utilities;
using System.Xml;

namespace ClearCanvas.Common.Configuration.Setup
{
	[ExtensionOf(typeof(ApplicationRootExtensionPoint))]
	internal class ImportSharedSettingsApplication : IApplicationRoot
	{
		private class CommandLine : Utilities.CommandLine
		{
			public CommandLine(string[] args)
				: base(args)
			{
			}

			[CommandLineParameter(0, "The name of the local file where settings should be imported from.", Required = true)]
			public string ConfigurationFilename { get; set; }
		}

		#region IApplicationRoot Members

		public void RunApplication(string[] args)
		{
			CommandLine commandLine = new CommandLine(args);
			foreach (SettingsGroupDescriptor group in SettingsGroupDescriptor.ListInstalledSettingsGroups(false))
			{
				Type type = Type.GetType(group.AssemblyQualifiedTypeName, true);
				var settings = ApplicationSettingsHelper.GetSettingsClassInstance(type);
				ApplicationSettingsExtensions.ImportSharedSettings(settings, commandLine.ConfigurationFilename);
			}
		}

		#endregion
	}
}
