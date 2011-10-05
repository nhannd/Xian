#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Authentication.Brokers;
using ClearCanvas.Common;
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
		/// <param name="addToGroups"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public IList<AuthorityToken> Import(IEnumerable<AuthorityTokenDefinition> tokenDefs,
			IList<string> addToGroups, IUpdateContext context)
		{
			// first load all the existing tokens into memory
			// there should not be that many tokens ( < 500), so this should not be a problem
			var broker = context.GetBroker<IAuthorityTokenBroker>();
			var existingTokens = broker.FindAll();

			// if there are groups to add to, load the groups
			var groups = addToGroups != null && addToGroups.Count > 0 ? LoadGroups(addToGroups, context) : new List<AuthorityGroup>();

			// order the input such that the renames are processed first
			// otherwise there may be a corner case where a newly imported token is immediately renamed
			tokenDefs = tokenDefs.OrderBy(t => t.FormerIdentities.Length > 0);

			foreach (var tokenDef in tokenDefs)
			{
				var token = ProcessToken(tokenDef, existingTokens, context);

				// add to groups
				CollectionUtils.ForEach(groups, g => g.AuthorityTokens.Add(token));
			}

			return existingTokens;
		}

		private static AuthorityToken ProcessToken(AuthorityTokenDefinition tokenDef, ICollection<AuthorityToken> existingTokens, IUpdateContext context)
		{
			// look for an existing instance of the token, or a token that should be renamed to this token
			var token = existingTokens.FirstOrDefault(t => t.Name == tokenDef.Token || tokenDef.FormerIdentities.Contains(t.Name));
			if (token != null)
			{
				// update the name (in the case it is a rename)
				token.Name = tokenDef.Token;

				// update the description
				token.Description = tokenDef.Description;
			}
			else
			{
				// the token does not already exist, so create it
				token = new AuthorityToken(tokenDef.Token, tokenDef.Description);
				context.Lock(token, DirtyState.New);
				existingTokens.Add(token);
			}
			return token;
		}

		private static IList<AuthorityGroup> LoadGroups(IEnumerable<string> groupNames, IPersistenceContext context)
		{
			var where = new AuthorityGroupSearchCriteria();
			where.Name.In(groupNames);

			return context.GetBroker<IAuthorityGroupBroker>().Find(where);
		}

		#region IApplicationRoot Members

		public void RunApplication(string[] args)
		{
			using (var scope = new PersistenceScope(PersistenceContextType.Update))
			{
				((IUpdateContext)PersistenceScope.CurrentContext).ChangeSetRecorder.OperationName = this.GetType().FullName;
				ImportFromPlugins((IUpdateContext)PersistenceScope.CurrentContext);

				scope.Complete();
			}
		}

		#endregion
	}
}
