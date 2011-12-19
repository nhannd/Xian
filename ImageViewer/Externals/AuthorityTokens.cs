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

namespace ClearCanvas.ImageViewer.Externals
{
    [ExtensionOf(typeof(DefineAuthorityGroupsExtensionPoint), Enabled = false)]
	internal class DefineAuthorityGroups : IDefineAuthorityGroups
	{
		#region IDefineAuthorityGroups Members

		/// <summary>
		/// Get the authority group definitions.
		/// </summary>
		public AuthorityGroupDefinition[] GetAuthorityGroups()
		{
			return new AuthorityGroupDefinition[]
			       	{
			       		new AuthorityGroupDefinition(DefaultAuthorityGroups.HealthcareAdministrators,
			       		                             new string[]
			       		                             	{
			       		                             		AuthorityTokens.Externals
			       		                             	}),
			       		new AuthorityGroupDefinition(DefaultAuthorityGroups.Radiologists,
			       		                             new string[]
			       		                             	{
			       		                             		AuthorityTokens.Externals
			       		                             	}),
			       		new AuthorityGroupDefinition(DefaultAuthorityGroups.RadiologyResidents,
			       		                             new string[]
			       		                             	{
			       		                             		AuthorityTokens.Externals
			       		                             	}),
			       		new AuthorityGroupDefinition(DefaultAuthorityGroups.RadiologyResidents,
			       		                             new string[]
			       		                             	{
			       		                             		AuthorityTokens.Externals
			       		                             	})
			       	};
		}

		#endregion
	}

	public static class AuthorityTokens
	{
		[AuthorityToken(Description = "Grant access to the External Applications feature.")]
		public const string Externals = "Viewer/Externals";
	}
}