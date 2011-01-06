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
using System.Reflection;
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
			List<AuthorityTokenDefinition> tokens = new List<AuthorityTokenDefinition>();
			// scan all plugins for token definitions
			foreach (PluginInfo plugin in Platform.PluginManager.Plugins)
			{
				IResourceResolver resolver = new ResourceResolver(plugin.Assembly);
				foreach (Type type in plugin.Assembly.GetTypes())
				{
					// look at public fields
					foreach (FieldInfo field in type.GetFields())
					{
						AuthorityTokenAttribute attr = AttributeUtils.GetAttribute<AuthorityTokenAttribute>(field, false);
						if (attr != null)
						{
							string token = (string)field.GetValue(null);
							string description = resolver.LocalizeString(attr.Description);

							tokens.Add(new AuthorityTokenDefinition(token, description));

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
