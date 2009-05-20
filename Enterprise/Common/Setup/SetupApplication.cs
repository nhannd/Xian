#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
				}
            }
			catch (CommandLineException e)
			{
				Console.WriteLine(e.Message);
			}
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
