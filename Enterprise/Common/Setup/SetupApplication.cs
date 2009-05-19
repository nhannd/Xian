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
using System.Text;
using ClearCanvas.Common;
using System.IO;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Enterprise.Common.Authentication;
using System.Net;
using System.Security.Principal;
using System.Threading;

namespace ClearCanvas.Enterprise.Common.Setup
{
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class SetupApplication : IApplicationRoot
    {

        #region Principal class

        class Principal : GenericPrincipal, IUserCredentialsProvider
        {
            private SessionToken _token;
            public Principal(IIdentity identity, SessionToken token, string[] roles)
                :base(identity, roles)
            {
                _token = token;
            }


            public string UserName
            {
                get { return this.Identity.Name; }
            }

            public string SessionTokenId
            {
                get { return _token.Id; }
            }
        }

        #endregion

        #region IApplicationRoot Members

        public void RunApplication(string[] args)
        {
            SetupCommandLine cmdLine = new SetupCommandLine();
            try
            {
                cmdLine.Parse(args);

                RemoteConnectionScope(cmdLine.UserName, cmdLine.Password,
                    delegate
                    {
                        ImportSettingsGroups();
                    });

            }
			catch (CommandLineException e)
			{
				Console.WriteLine(e.Message);
			}
        }

        #endregion

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

        private void RemoteConnectionScope(string user, string password, Action<object> action)
        {
            SessionToken sessionToken = null;
            Platform.GetService<IAuthenticationService>(
                delegate(IAuthenticationService service)
                {
                    // obtain session
                    InitiateSessionResponse response = service.InitiateSession(
                        new InitiateSessionRequest(user, "setup", Dns.GetHostName(), password, true));

                    // establish thread principal
                    sessionToken = response.SessionToken;
                    Thread.CurrentPrincipal = new Principal(
                        new GenericIdentity(user),
                        sessionToken,
                        response.AuthorityTokens);

                    try
                    {
                        // execute actions
                        action(null);
                    }
                    finally
                    {
                        // terminate session
                        service.TerminateSession(
                            new TerminateSessionRequest(user, sessionToken));
                    }
                });
        }
    }
}
