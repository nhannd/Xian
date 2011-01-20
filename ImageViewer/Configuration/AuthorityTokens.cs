#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Common.Authorization;

namespace ClearCanvas.ImageViewer.Configuration
{
	[ExtensionOf(typeof(DefineAuthorityGroupsExtensionPoint))]
	internal class DefineAuthorityGroups : IDefineAuthorityGroups
	{
		#region IDefineAuthorityGroups Members

		public AuthorityGroupDefinition[] GetAuthorityGroups()
		{
			return new AuthorityGroupDefinition[]
            {
                new AuthorityGroupDefinition(DefaultAuthorityGroups.HealthcareAdministrators,
				    new string[] 
				    {
						AuthorityTokens.Configuration.DefaultServers
				   }),

				new AuthorityGroupDefinition(DefaultAuthorityGroups.Technologists,
				    new string[] 
				    {
						AuthorityTokens.Configuration.DefaultServers
				   }),

                new AuthorityGroupDefinition(DefaultAuthorityGroups.Radiologists,
				    new string[] 
				    {
						AuthorityTokens.Configuration.DefaultServers
				   }),

                new AuthorityGroupDefinition(DefaultAuthorityGroups.RadiologyResidents,
				    new string[] 
				    {
						AuthorityTokens.Configuration.DefaultServers
				   }),

                new AuthorityGroupDefinition(DefaultAuthorityGroups.EmergencyPhysicians,
				    new string[] 
				    {
						AuthorityTokens.Configuration.DefaultServers
				   })
            };
		}

		#endregion
	}

	public static class AuthorityTokens
	{
		public static class Configuration
		{
			[AuthorityToken(Description = "Allow configuration of default servers.")]
			public const string DefaultServers = "Viewer/Configuration/Default Servers";
		}
	}
}
