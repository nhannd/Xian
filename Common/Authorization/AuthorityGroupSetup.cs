#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common.Authorization
{
	/// <summary>
	/// Extension point for defining default authority groups to be imported at deployment time.
	/// </summary>
	[ExtensionPoint]
	public sealed class DefineAuthorityGroupsExtensionPoint : ExtensionPoint<IDefineAuthorityGroups>
	{
	}

	/// <summary>
	/// Helper class for setting up authority groups.
	/// </summary>
	public static class AuthorityGroupSetup
	{
		/// <summary>
		/// Returns the set of authority tokens defined by all plugins.
		/// </summary>
		/// <returns></returns>
		public static AuthorityTokenDefinition[] GetAuthorityTokens()
		{
			var tokens = new List<AuthorityTokenDefinition>();
			// scan all plugins for token definitions
			foreach (var plugin in Platform.PluginManager.Plugins)
			{
				var resolver = new ResourceResolver(plugin.Assembly);
				foreach (var type in plugin.Assembly.GetTypes())
				{
					// look at public fields
					foreach (var field in type.GetFields())
					{
						var attr = AttributeUtils.GetAttribute<AuthorityTokenAttribute>(field, false);
						if (attr != null)
						{
							var token = (string)field.GetValue(null);
							var description = resolver.LocalizeString(attr.Description);
							var formerIdentities = (attr.Formerly ?? "").Split(';');

							tokens.Add(new AuthorityTokenDefinition(token, plugin.Assembly.FullName, description, formerIdentities));
						}
					}
				}
			}
			return tokens.ToArray();
		}


		/// <summary>
		/// Returns the set of default authority groups defined by all plugins.
		/// </summary>
		/// <remarks>
		/// The default authority groups are only be used at deployment time to initialize the authorization system.
		/// They do not reflect the actual set of authority groups that exist for a given deployment.
		/// </remarks>
		/// <returns></returns>
		public static AuthorityGroupDefinition[] GetDefaultAuthorityGroups()
		{
			var groupDefs = new List<AuthorityGroupDefinition>();
			foreach (IDefineAuthorityGroups groupDefiner in new DefineAuthorityGroupsExtensionPoint().CreateExtensions())
			{
				var groups = groupDefiner.GetAuthorityGroups();
				if (groups != null)
				{
					groupDefs.AddRange(groups);
				}
			}
			return groupDefs.ToArray();
		}
	}
}
