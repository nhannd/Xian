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
using ClearCanvas.Common;
using ClearCanvas.Common.Authorization;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;

namespace ClearCanvas.Enterprise.Common.Setup
{
	public static class SetupHelper
	{
		/// <summary>
		/// Import authority tokens defined in local plugins.
		/// </summary>
		public static void ImportAuthorityTokens(string[] addToGroups)
		{
			var tokens = AuthorityGroupSetup.GetAuthorityTokens();
			var summaries = tokens.Select(t => new AuthorityTokenSummary(t.Token, t.DefiningAssembly, t.Description, t.FormerIdentities)).ToList();

			Platform.GetService<IAuthorityGroupAdminService>(
				service => service.ImportAuthorityTokens(new ImportAuthorityTokensRequest(summaries, new List<string>(addToGroups))));

			LogImportedTokens(tokens);
		}

		/// <summary>
		/// Import authority groups defined in local plugins.
		/// </summary>
		public static void ImportAuthorityGroups()
		{

			var groups = AuthorityGroupSetup.GetDefaultAuthorityGroups();
			var groupDetails = groups.Select(g =>
				new AuthorityGroupDetail(
					null,
					g.Name,
					g.Description,
					g.DataGroup,
					g.Tokens.Select(t => new AuthorityTokenSummary(t)).ToList()
				)).ToList();

			Platform.GetService<IAuthorityGroupAdminService>(
				service => service.ImportAuthorityGroups(new ImportAuthorityGroupsRequest(groupDetails)));

			LogImportedDefaultGroups(groups);
		}

		private static void LogImportedDefaultGroups(IEnumerable<AuthorityGroupDefinition> groups)
		{
			foreach (var g in groups.Distinct())
			{
				Platform.Log(LogLevel.Info, "Imported default authority group definition: {0}", g.Name);
			}
		}

		private static void LogImportedTokens(IEnumerable<AuthorityTokenDefinition> tokens)
		{
			foreach (var token in tokens)
			{
				Platform.Log(LogLevel.Info, "Imported authority token '{0}' from {1}", token.Token, token.DefiningAssembly);
			}
		}

	}
}
