#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common.Utilities;
using System.Xml;

namespace ClearCanvas.Common.Configuration.Setup
{
	[ExtensionOf(typeof(ApplicationRootExtensionPoint))]
	internal class ImportLocalSharedSettingsApplication : IApplicationRoot
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

			foreach (SettingsGroupDescriptor group in SettingsGroupDescriptor.ListInstalledLocalSettingsGroups())
			{
				Type type = Type.GetType(group.AssemblyQualifiedTypeName, true);
				var settings = ApplicationSettingsHelper.GetSettingsClassInstance(type);
				ApplicationSettingsExtensions.ImportSharedSettings(settings, commandLine.ConfigurationFilename);
			}
		}

		#endregion
	}
}
