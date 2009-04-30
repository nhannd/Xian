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
using System.Reflection;
using System.Text;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Authorization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;
using ClearCanvas.ImageServer.Enterprise.Admin;
using ClearCanvas.ImageServer.Enterprise.Authentication;

namespace ClearCanvas.ImageServer.Utilities
{
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class AuthorityGroupImporter : IApplicationRoot
    {
        SessionInfo _session;
        #region IApplicationRoot Members

        public void RunApplication(string[] args)
        {
            CommandLine cmd = new CommandLine(args);
            string user = cmd.Named["u"];
            string pwd = cmd.Named["p"];

            using(LoginService service = new LoginService())
            {
               try
               {
                   Console.WriteLine("Login as {0}...", user);
                   _session = service.Login(user, pwd);
               }
               catch (PasswordExpiredException)
               {
                   Console.WriteLine("Password has expired. Reseting password..");
                   service.ChangePassword(user, pwd, "clearcanvas123");
                   service.ChangePassword(user, "clearcanvas123", pwd);
                   Console.WriteLine("Attempt to login again...");
                   _session = service.Login(user, pwd);
               }
            }

            Thread.CurrentPrincipal = _session.User;
            AuthorityGroupDefinition[] groupDefs = AuthorityGroupSetup.GetDefaultAuthorityGroups();
            Import(groupDefs);
        }

        #endregion

        private void Import(IEnumerable<AuthorityGroupDefinition> groupDefs)
        {
            List<AuthorityGroupDetail> groups = CollectionUtils.Map<AuthorityGroupDefinition, AuthorityGroupDetail>(
                groupDefs,
                delegate(AuthorityGroupDefinition groupDef)
                {
                    AuthorityGroupDetail group = new AuthorityGroupDetail();
                    group.Name = groupDef.Name;
                    group.AuthorityTokens = CollectionUtils.Map<string, AuthorityTokenSummary>(
                        groupDef.Tokens,
                        delegate(string tokenName)
                            {
                                AuthorityTokenSummary token = new AuthorityTokenSummary(tokenName, String.Empty);
                                return token;
                            });
                    return group;
                }
                );

            Console.WriteLine("*********************************************");
            Console.WriteLine("The following groups will be created:");
            foreach(AuthorityGroupDetail group in groups)
            {
                Console.WriteLine(group.Name);
            }
            Console.WriteLine("*********************************************");

            using(AuthorityManagement service = new AuthorityManagement())
            {
                Console.WriteLine("Loading default groups into the server...");
                if (!service.ImportAuthorityGroups(groups))
                    Console.WriteLine("Unable to load default user groups. See server log for more info.");
                else
                    Console.WriteLine("Groups are successfully loaded.");

            };
        }

    }
}
