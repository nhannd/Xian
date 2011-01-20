#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Authorization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;
using System.Net;

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
            SetupCommandLine cmdLine = new SetupCommandLine();
            try
            {
                cmdLine.Parse(args);

				using(new AuthenticationScope(cmdLine.UserName, "setup", Dns.GetHostName(), cmdLine.Password))
				{
					// first import the tokens, since the default groups will likely depend on these tokens
                    if (cmdLine.ImportAuthorityTokens)
                    {
                        ImportAuthorityTokens(cmdLine.SysAdminGroup);
                    }

					// import authority groups
					if(cmdLine.ImportDefaultAuthorityGroups)
					{
						ImportAuthorityGroups();
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

		private void MigrateSharedSettings(string previousExeConfigFilename)
		{
			foreach (SettingsGroupDescriptor group in SettingsGroupDescriptor.ListInstalledSettingsGroups(false))
				SettingsMigrator.MigrateSharedSettings(group, previousExeConfigFilename);
		}

        #endregion

		/// <summary>
		/// Import settings groups defined in local plugins.
		/// </summary>
        private static void ImportSettingsGroups()
        {
            List<SettingsGroupDescriptor> groups = SettingsGroupDescriptor.ListInstalledSettingsGroups(true);
            Platform.GetService<Configuration.IConfigurationService>(
                delegate(Configuration.IConfigurationService service)
                {
                    foreach (SettingsGroupDescriptor group in groups)
                    {
                        List<SettingsPropertyDescriptor> props = SettingsPropertyDescriptor.ListSettingsProperties(group);
                        service.ImportSettingsGroup(
                            new Configuration.ImportSettingsGroupRequest(group, props));
                    }
                });
        }

		/// <summary>
		/// Import authority tokens defined in local plugins.
		/// </summary>
		private static void ImportAuthorityTokens(string sysAdminGroup)
		{
			string[] addToGroups = string.IsNullOrEmpty(sysAdminGroup) ? new string[] { } : new string[] { sysAdminGroup };

			AuthorityTokenDefinition[] tokens = AuthorityGroupSetup.GetAuthorityTokens();

			List<AuthorityTokenSummary> summaries = CollectionUtils.Map<AuthorityTokenDefinition, AuthorityTokenSummary>(tokens,
								delegate(AuthorityTokenDefinition t)
								{
									return new AuthorityTokenSummary(t.Token, t.Description);
								});

			Platform.GetService<IAuthorityGroupAdminService>(
				delegate(IAuthorityGroupAdminService service)
				{
					service.ImportAuthorityTokens(
						new ImportAuthorityTokensRequest(summaries, new List<string>(addToGroups)));
				});
		}

		/// <summary>
		/// Import authority groups defined in local plugins.
		/// </summary>
		private static void ImportAuthorityGroups()
		{
			AuthorityGroupDefinition[] groups = AuthorityGroupSetup.GetDefaultAuthorityGroups();

			Platform.GetService<IAuthorityGroupAdminService>(
				delegate(IAuthorityGroupAdminService service)
				{
					service.ImportAuthorityGroups(
						new ImportAuthorityGroupsRequest(
							CollectionUtils.Map<AuthorityGroupDefinition, AuthorityGroupDetail>(groups,
								delegate(AuthorityGroupDefinition g)
								{
									return new AuthorityGroupDetail(null, g.Name,
										CollectionUtils.Map<string, AuthorityTokenSummary>(g.Tokens,
											delegate(string t) { return new AuthorityTokenSummary(t, null); }));
								})));
				});
		}
    }
}
