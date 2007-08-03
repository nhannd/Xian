using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Authorization
{
    /// <summary>
    /// Interface used by <see cref="DefineAuthorityGroupsExtensionPoint"/>. 
    /// </summary>
    public interface IDefineAuthorityGroups
    {
        /// <summary>
        /// Get the authority group definitions.
        /// </summary>
        /// <returns></returns>
        AuthorityGroupDefinition[] GetAuthorityGroups();
    }
}
