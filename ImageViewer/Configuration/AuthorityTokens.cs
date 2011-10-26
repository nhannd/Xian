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
    [ExtensionOf(typeof(DefineAuthorityGroupsExtensionPoint), Enabled = false)]
	internal class DefineAuthorityGroups : IDefineAuthorityGroups
	{
		#region IDefineAuthorityGroups Members

		public AuthorityGroupDefinition[] GetAuthorityGroups()
		{
			return new AuthorityGroupDefinition[]
            {
                new AuthorityGroupDefinition(DefaultAuthorityGroups.HealthcareAdministrators,
                    DefaultAuthorityGroups.HealthcareAdministrators,
                    false,
				    new string[] 
				    {
						AuthorityTokens.Configuration.DefaultServers
				   }),

				new AuthorityGroupDefinition(DefaultAuthorityGroups.Technologists,
                    DefaultAuthorityGroups.Technologists,
                    false,
				    new string[] 
				    {
						AuthorityTokens.Configuration.DefaultServers
				   }),

                new AuthorityGroupDefinition(DefaultAuthorityGroups.Radiologists,
                    DefaultAuthorityGroups.Radiologists,
                    false,
				    new string[] 
				    {
						AuthorityTokens.Configuration.DefaultServers
				   }),

                new AuthorityGroupDefinition(DefaultAuthorityGroups.RadiologyResidents,
                    DefaultAuthorityGroups.RadiologyResidents,
                    false,
				    new string[] 
				    {
						AuthorityTokens.Configuration.DefaultServers
				   }),

                new AuthorityGroupDefinition(DefaultAuthorityGroups.EmergencyPhysicians,
                    DefaultAuthorityGroups.EmergencyPhysicians,
                    false,
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
		[AuthorityToken(Description = "Allow publishing of locally created data to remote servers.")]
		public const string Publishing = "Viewer/Publishing";

		public static class Configuration
		{
			[AuthorityToken(Description = "Allow configuration of default servers.")]
			public const string DefaultServers = "Viewer/Configuration/Default Servers";

		    [AuthorityToken(Description = "Allow configuration of data publishing options.", Formerly = "Viewer/Administration/Key Images")]
			public const string Publishing = "Viewer/Configuration/Publishing";
		}
	}
}
