using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Authorization
{
    /// <summary>
    /// Extension point for defining default authority groups to be imported at deployment time.
    /// </summary>
    [ExtensionPoint]
    public class DefineAuthorityGroupsExtensionPoint : ExtensionPoint<IDefineAuthorityGroups>
    {
    }

    public static class AuthorityGroupSetup
    {
        /// <summary>
        /// Returns the set of default authority groups defined by all plugins.
        /// </summary>
        /// <remarks>
        /// The default authority groups should only be used at deployment time to initialize the authorization system.
        /// They do not reflect the actual set of authority groups that exist for a given deployment.
        /// </remarks>
        /// <returns></returns>
        public static AuthorityGroupDefinition[] GetDefaultAuthorityGroups()
        {
            List<AuthorityGroupDefinition> groupDefs = new List<AuthorityGroupDefinition>();
            foreach (IDefineAuthorityGroups groupDefiner in new DefineAuthorityGroupsExtensionPoint().CreateExtensions())
            {
                groupDefs.AddRange(groupDefiner.GetAuthorityGroups());
            }
            return groupDefs.ToArray();
        }
    }
}
