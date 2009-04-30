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
using System.Xml;
using ClearCanvas.Enterprise.Core;
using System.IO;
using ClearCanvas.Enterprise.Authentication.Brokers;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using ClearCanvas.Common.Authorization;

namespace ClearCanvas.Enterprise.Authentication.Imex
{

    /// <summary>
    /// Imports authority groups from plugins that define extensions to <see cref="DefineAuthorityGroupsExtensionPoint"/>.
    /// </summary>
    /// <remarks>
    /// This class implements <see cref="IApplicationRoot"/> so that it may be run stand-alone from a console.  However,
    /// it may also be used as a utility class to be invoked by other means.
    /// </remarks>
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class AuthorityGroupImporter : IApplicationRoot
    {
        /// <summary>
        /// Import authority groups from extensions of <see cref="DefineAuthorityGroupsExtensionPoint"/>.
        /// </summary>
        /// <remarks>
        /// Creates any authority groups that do not already exist.
        /// This method performs an additive import.  It will never remove an existing authority group or
        /// remove authority tokens from an existing group.
        /// </remarks>
        /// <param name="context"></param>
        public IList<AuthorityGroup> ImportFromPlugins(IUpdateContext context)
        {
            AuthorityGroupDefinition[] groupDefs = AuthorityGroupSetup.GetDefaultAuthorityGroups();
            return Import(groupDefs, context);
        }

		/// <summary>
		/// Import authority groups.
		/// </summary>
		/// <remarks>
		/// Creates any authority groups that do not already exist.
		/// This method performs an additive import.  It will never remove an existing authority group or
		/// remove authority tokens from an existing group.
		/// </remarks>
		/// <param name="groupDefs"></param>
		/// <param name="context"></param>
		public IList<AuthorityGroup> Import(IEnumerable<AuthorityGroupDefinition> groupDefs, IUpdateContext context)
        {
            // first load all the existing tokens into memory
            // there should not be that many tokens ( < 500), so this should not be a problem
            IAuthorityTokenBroker tokenBroker = context.GetBroker<IAuthorityTokenBroker>();
            IList<AuthorityToken> existingTokens = tokenBroker.FindAll();

            // load existing groups
            IAuthorityGroupBroker groupBroker = context.GetBroker<IAuthorityGroupBroker>();
            IList<AuthorityGroup> existingGroups = groupBroker.FindAll();

            foreach (AuthorityGroupDefinition groupDef in groupDefs)
            {
                AuthorityGroup group = CollectionUtils.SelectFirst(existingGroups,
                    delegate(AuthorityGroup g) { return g.Name == groupDef.Name; });

                // if group does not exist, create it
                if (group == null)
                {
                    group = new AuthorityGroup();
                    group.Name = groupDef.Name;
                    context.Lock(group, DirtyState.New);
                    existingGroups.Add(group);
                }

                // process all token nodes contained in group
                foreach (string tokenName in groupDef.Tokens)
                {
                    AuthorityToken token = CollectionUtils.SelectFirst(existingTokens,
                        delegate(AuthorityToken t) { return t.Name == tokenName; });

                    // ignore non-existent tokens
                    if (token == null)
                        continue;

                    // add the token to the group
                    group.AuthorityTokens.Add(token);
                }
            }

            return existingGroups;
        }

        #region IApplicationRoot Members

        public void RunApplication(string[] args)
        {
            using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Update))
            {
                ((IUpdateContext) PersistenceScope.CurrentContext).ChangeSetRecorder.OperationName = this.GetType().FullName;
                ImportFromPlugins((IUpdateContext)PersistenceScope.CurrentContext);

                scope.Complete();
            }
        }

        #endregion

    }
}
