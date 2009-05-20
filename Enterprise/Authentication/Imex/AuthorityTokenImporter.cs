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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Authentication.Brokers;
using System.IO;
using ClearCanvas.Common;
using System.Reflection;
using ClearCanvas.Common.Authorization;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Authentication.Imex
{
    /// <summary>
    /// Scans the entire set of installed plugins for declared authority tokens (const string fields marked
    /// with the <see cref="AuthorityTokenAttribute"/>), and imports them into the database.
    /// </summary>
    /// <remarks>
    /// This class implements <see cref="IApplicationRoot"/> so that it may be run stand-alone from a console.  However,
    /// it may also be used as a utility class to be invoked by other means.
    /// </remarks>
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class AuthorityTokenImporter : IApplicationRoot
    {
        /// <summary>
        /// Imports authority tokens from all installed plugins.
        /// </summary>
        /// <param name="context">Persistence context</param>
        /// <returns>A complete list of all existing authority tokens (including any that existed prior to this import).</returns>
        public IList<AuthorityToken> ImportFromPlugins(IUpdateContext context)
        {
            // scan all plugins for token definitions
        	AuthorityTokenDefinition[] tokenDefs = AuthorityGroupSetup.GetAuthorityTokens();
        	return Import(tokenDefs, null, context);
        }

		/// <summary>
		/// Imports the specified set of authority tokens.
		/// </summary>
		/// <param name="tokenDefs"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public IList<AuthorityToken> Import(IEnumerable<AuthorityTokenDefinition> tokenDefs,
            IList<string> addToGroups, IUpdateContext context)
		{
			// first load all the existing tokens into memory
			// there should not be that many tokens ( < 500), so this should not be a problem
			IAuthorityTokenBroker broker = context.GetBroker<IAuthorityTokenBroker>();
			IList<AuthorityToken> existingTokens = broker.FindAll();

            // if there are groups to add to, load the groups
            IList<AuthorityGroup> groups = addToGroups != null && addToGroups.Count > 0
                ? LoadGroups(addToGroups, context) : new List<AuthorityGroup>();

			foreach (AuthorityTokenDefinition tokenDef in tokenDefs)
			{
				AuthorityToken token = ProcessToken(tokenDef, existingTokens, context);
				existingTokens.Add(token);

                // add to groups
                CollectionUtils.ForEach(groups, delegate(AuthorityGroup g) { g.AuthorityTokens.Add(token); });
			}

			return existingTokens;
		}

		private static AuthorityToken ProcessToken(AuthorityTokenDefinition tokenDef, IList<AuthorityToken> existingTokens, IUpdateContext context)
        {
            AuthorityToken token = CollectionUtils.SelectFirst(existingTokens,
                delegate(AuthorityToken t) { return t.Name == tokenDef.Token; });

            // if token does not already exist, create it
            if (token == null)
            {
                token = new AuthorityToken();
                token.Name = tokenDef.Token;

                context.Lock(token, DirtyState.New);
            }

            // update the description
			token.Description = tokenDef.Description;

            return token;
        }

        private IList<AuthorityGroup> LoadGroups(IEnumerable<string> groupNames, IPersistenceContext context)
        {
            AuthorityGroupSearchCriteria where = new AuthorityGroupSearchCriteria();
            where.Name.In(groupNames);

            return context.GetBroker<IAuthorityGroupBroker>().Find(where);
        }

        #region IApplicationRoot Members

        public void RunApplication(string[] args)
        {
            using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Update))
            {
                ((IUpdateContext)PersistenceScope.CurrentContext).ChangeSetRecorder.OperationName = this.GetType().FullName;
                ImportFromPlugins((IUpdateContext)PersistenceScope.CurrentContext);

                scope.Complete();
            }
        }

        #endregion
    }
}
