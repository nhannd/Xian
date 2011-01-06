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
