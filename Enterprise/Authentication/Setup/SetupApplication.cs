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
					((IUpdateContext)PersistenceScope.CurrentContext).ChangeSetRecorder.OperationName = GetType().FullName;

					// import authority tokens
					AuthorityTokenImporter tokenImporter = new AuthorityTokenImporter();
					IList<AuthorityToken> allTokens = tokenImporter.ImportFromPlugins((IUpdateContext)PersistenceScope.CurrentContext);


					// create the sys admin group, which has all tokens assigned by default
					string[] tokenStrings = CollectionUtils.Map<AuthorityToken, string, List<string>>(allTokens,
					                                                                                  t => t.Name).ToArray();
                    AuthorityGroupDefinition adminGroupDef = new AuthorityGroupDefinition(cmdLine.SysAdminGroup, cmdLine.SysAdminGroup, false,
				                                                                          tokenStrings);
					AuthorityGroupImporter groupImporter = new AuthorityGroupImporter();

					IList<AuthorityGroup> groups = 
						groupImporter.Import(new AuthorityGroupDefinition[] { adminGroupDef }, (IUpdateContext)PersistenceScope.CurrentContext);

					// find the admin group entity that was just created
					AuthorityGroup adminGroup = CollectionUtils.SelectFirst(groups,
					                                                        g => g.Name == cmdLine.SysAdminGroup);

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

        private static void CreateSysAdminUser(AuthorityGroup adminGroup, SetupCommandLine cmdLine, IPersistenceContext context, TextWriter log)
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
                HashedSet<AuthorityGroup> groups = new HashedSet<AuthorityGroup>
                                                       {
                                                           adminGroup
                                                       };

                // create sa user using initial password, set to expire never
                User saUser = User.CreateNewUser(
					new UserInfo(cmdLine.SysAdminUserName, cmdLine.SysAdminDisplayName, null, null, null),
					Password.CreatePassword(cmdLine.SysAdminInitialPassword, null),
                    groups);
                context.Lock(saUser, DirtyState.New);
            }
        }

        #endregion
    }
}
