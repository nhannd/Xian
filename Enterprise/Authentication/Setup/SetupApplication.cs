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
using ClearCanvas.Enterprise.Authentication.Imex;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Authentication.Brokers;
using System.IO;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common.Authorization;
using Iesi.Collections.Generic;

namespace ClearCanvas.Enterprise.Authentication.Setup
{
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

				using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Update))
				{
					((IUpdateContext)PersistenceScope.CurrentContext).ChangeSetRecorder.OperationName = this.GetType().FullName;

					// import authority tokens
					AuthorityTokenImporter tokenImporter = new AuthorityTokenImporter();
					IList<AuthorityToken> allTokens = tokenImporter.ImportFromPlugins((IUpdateContext)PersistenceScope.CurrentContext);


					// create the sys admin group, which has all tokens assigned by default
					string[] tokenStrings = CollectionUtils.Map<AuthorityToken, string, List<string>>(allTokens,
					   delegate(AuthorityToken t) { return t.Name; }).ToArray();
					AuthorityGroupDefinition adminGroupDef = new AuthorityGroupDefinition(cmdLine.SysAdminGroup, tokenStrings);
					AuthorityGroupImporter groupImporter = new AuthorityGroupImporter();

					IList<AuthorityGroup> groups = 
						groupImporter.Import(new AuthorityGroupDefinition[] { adminGroupDef }, (IUpdateContext)PersistenceScope.CurrentContext);

					// find the admin group entity that was just created
					AuthorityGroup adminGroup = CollectionUtils.SelectFirst(groups,
						delegate(AuthorityGroup g) { return g.Name == cmdLine.SysAdminGroup; });

					// create the "sa" user
					CreateSysAdminUser(adminGroup, cmdLine, PersistenceScope.CurrentContext, Console.Out);

					// optionally import other default authority groups defined in other plugins
					if (cmdLine.ImportDefaultAuthorityGroups)
					{
						groupImporter.ImportFromPlugins((IUpdateContext) PersistenceScope.CurrentContext);
					}

					scope.Complete();
				}
			}
			catch (CommandLineException e)
			{
				Console.WriteLine(e.Message);
				cmdLine.PrintUsage(Console.Out);
			}
        }

        private void CreateSysAdminUser(AuthorityGroup adminGroup, SetupCommandLine cmdLine, IPersistenceContext context, TextWriter log)
        {
            try
            {
                // create the sa user, if doesn't already exist
                IUserBroker userBroker = context.GetBroker<IUserBroker>();
                UserSearchCriteria where = new UserSearchCriteria();
				where.UserName.EqualTo(cmdLine.SysAdminUserName);
                userBroker.FindOne(where);

				log.WriteLine(string.Format("User '{0}' already exists.", cmdLine.SysAdminUserName));
            }
            catch (EntityNotFoundException)
            {
                HashedSet<AuthorityGroup> groups = new HashedSet<AuthorityGroup>();
                groups.Add(adminGroup);

                // create sa user using initial password, set to expire immediately
                // (note that DateTime.Now is used instead of Platform.Time because
                // we don't want to assume there is a time-server during setup)
                User saUser = User.CreateNewUser(
					new UserInfo(cmdLine.SysAdminUserName, cmdLine.SysAdminDisplayName, null, null),
					Password.CreatePassword(cmdLine.SysAdminInitialPassword, DateTime.Now),
                    groups);
                context.Lock(saUser, DirtyState.New);
            }
        }

        #endregion
    }
}
