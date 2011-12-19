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

namespace ClearCanvas.ImageViewer.Tools.Reporting
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
						AuthorityTokens.KeyImages
				   }),

                new AuthorityGroupDefinition(DefaultAuthorityGroups.Radiologists,
				    new string[] 
				    {
						AuthorityTokens.KeyImages
				   }),

                new AuthorityGroupDefinition(DefaultAuthorityGroups.RadiologyResidents,
				    new string[] 
				    {
						AuthorityTokens.KeyImages
				   })
            };
		}

		#endregion
	}
	
	public static class AuthorityTokens
	{
		[AuthorityToken(Description = "Grant access to key image functionality.", Formerly = "Viewer/Reporting/Key Images")]
		public const string KeyImages = "Viewer/Study/Key Images";
	}
}
