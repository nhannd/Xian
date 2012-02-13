#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Net;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Enterprise.Common.Setup
{
	/// <summary>
	/// Connects to the enterprise server and imports settings groups, authority tokens, and authority groups.
	/// </summary>
	[ExtensionOf(typeof(ApplicationRootExtensionPoint))]
	public class SetupApplication : IApplicationRoot
	{
		#region IApplicationRoot Members

		public void RunApplication(string[] args)
		{
			var cmdLine = new SetupCommandLine();
			try
			{
				cmdLine.Parse(args);

				using (new AuthenticationScope(cmdLine.UserName, "setup", Dns.GetHostName(), cmdLine.Password))
				{
					// first import the tokens, since the default groups will likely depend on these tokens
					if (cmdLine.ImportAuthorityTokens)
					{
						var addToGroups = string.IsNullOrEmpty(cmdLine.SysAdminGroup) ? new string[] { } : new[] { cmdLine.SysAdminGroup };
						SetupHelper.ImportAuthorityTokens(addToGroups);
					}

					// import authority groups
					if (cmdLine.ImportDefaultAuthorityGroups)
					{
						SetupHelper.ImportAuthorityGroups();
					}

					// import settings groups
					if (cmdLine.ImportSettingsGroups)
					{
						ImportSettingsGroups();
					}

					if (cmdLine.MigrateSharedSettings)
					{
						MigrateSharedSettings(cmdLine.PreviousExeConfigFilename);
					}
				}
			}
			catch (CommandLineException e)
			{
				Console.WriteLine(e.Message);
			}
		}

		#endregion

		private static void MigrateSharedSettings(string previousExeConfigFilename)
		{
			foreach (var group in SettingsGroupDescriptor.ListInstalledSettingsGroups(false))
				SettingsMigrator.MigrateSharedSettings(group, previousExeConfigFilename);
		}

		/// <summary>
		/// Import settings groups defined in local plugins.
		/// </summary>
		private static void ImportSettingsGroups()
		{
			var groups = SettingsGroupDescriptor.ListInstalledSettingsGroups(true);

			foreach (var g in groups)
			{
				Platform.Log(LogLevel.Info, "Import settings group {0}, Version={1}, Type={2}", g.Name, g.Version.ToString(), g.AssemblyQualifiedTypeName);
			}

			Platform.GetService(
				delegate(Configuration.IConfigurationService service)
				{
					foreach (var group in groups)
					{
						var props = SettingsPropertyDescriptor.ListSettingsProperties(group);
						service.ImportSettingsGroup(new Configuration.ImportSettingsGroupRequest(group, props));
					}
				});
		}

	}
}
