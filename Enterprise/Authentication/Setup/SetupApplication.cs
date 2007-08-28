using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Authentication.Brokers;
using System.IO;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common.Authorization;

namespace ClearCanvas.Enterprise.Authentication.Setup
{
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class SetupApplication : IApplicationRoot
    {
        private const string SysAdminUserName = "sa";
        private const string SysAdminGroup = "Administrators";

        #region IApplicationRoot Members

        public void RunApplication(string[] args)
        {
            using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Update))
            {
                // import authority tokens
                AuthorityTokenImporter tokenImporter = new AuthorityTokenImporter();
                IList<AuthorityToken> allTokens = tokenImporter.ImportFromPlugins((IUpdateContext)PersistenceScope.Current, Console.Out);


                // create the sys admin group, which has all tokens assigned by default
                string[] tokenStrings = CollectionUtils.Map<AuthorityToken, string, List<string>>(allTokens,
                   delegate(AuthorityToken t) { return t.Name; }).ToArray();
                AuthorityGroupDefinition adminGroupDef = new AuthorityGroupDefinition(SysAdminGroup, tokenStrings);
                AuthorityGroupImporter groupImporter = new AuthorityGroupImporter();
                groupImporter.Import(new AuthorityGroupDefinition[] { adminGroupDef }, (IUpdateContext)PersistenceScope.Current, Console.Out);

                // import any other authority groups defined in other plugins
                IList<AuthorityGroup> allGroups = groupImporter.ImportFromPlugins((IUpdateContext)PersistenceScope.Current, Console.Out);

                // find the admin group that was just created
                AuthorityGroup adminGroup = CollectionUtils.SelectFirst<AuthorityGroup>(allGroups,
                    delegate(AuthorityGroup g) { return g.Name == SysAdminGroup; });

                // create the "sa" user
                CreateSysAdminUser(adminGroup, PersistenceScope.Current, Console.Out);

                scope.Complete();
            }
        }

        public void CreateSysAdminUser(AuthorityGroup adminGroup, IPersistenceContext context, TextWriter log)
        {
            try
            {
                // create the sa user, if doesn't already exist
                IUserBroker userBroker = context.GetBroker<IUserBroker>();
                UserSearchCriteria where = new UserSearchCriteria();
                where.UserName.EqualTo(SysAdminUserName);
                userBroker.FindOne(where);

                log.WriteLine(string.Format("User '{0}' already exists.", SysAdminUserName));
            }
            catch (EntityNotFoundException)
            {
                User saUser = new User();
                saUser.UserName = SysAdminUserName;
                saUser.AuthorityGroups.Add(adminGroup);

                context.Lock(saUser, DirtyState.New);
            }
        }

        #endregion
    }
}
