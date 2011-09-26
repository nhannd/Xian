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

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Default authority groups.
	/// </summary>
	public class DefaultAuthorityGroups
	{
		/// <summary>
		/// Healthcare Administrators authority group.
		/// </summary>
		public const string HealthcareAdministrators = "Healthcare Administrators";

		/// <summary>
		/// Clerical authority group.
		/// </summary>
		public const string Clerical = "Clerical";

		/// <summary>
		/// Technologists authority group.
		/// </summary>
		public const string Technologists = "Technologists";

		/// <summary>
		/// Radiologists authority group.
		/// </summary>
		public const string Radiologists = "Radiologists";

		/// <summary>
		/// Radiology Residents authority group.
		/// </summary>
		public const string RadiologyResidents = "Radiology Residents";

		/// <summary>
		/// Emergency Physicians authority group.
		/// </summary>
		public const string EmergencyPhysicians = "Emergency Physicians";
	}

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
		                                                DefaultAuthorityGroups.HealthcareAdministrators,
		                                                false,
		                                                new string[]
		                                                    {
		                                                        AuthorityTokens.ViewerVisible,
		                                                        AuthorityTokens.Study.Open
		                                                    }),

		                   new AuthorityGroupDefinition(DefaultAuthorityGroups.Clerical,
		                                                DefaultAuthorityGroups.Clerical,
		                                                false,
		                                                new string[]
		                                                    {
		                                                        AuthorityTokens.ViewerVisible
		                                                    }),

		                   new AuthorityGroupDefinition(DefaultAuthorityGroups.Technologists,
		                                                DefaultAuthorityGroups.Technologists,
		                                                false,
		                                                new string[]
		                                                    {
		                                                        AuthorityTokens.ViewerVisible,
		                                                        AuthorityTokens.Study.Open
		                                                    }),

		                   new AuthorityGroupDefinition(DefaultAuthorityGroups.Radiologists,
		                                                DefaultAuthorityGroups.Radiologists,
		                                                false,
		                                                new string[]
		                                                    {
		                                                        AuthorityTokens.ViewerVisible,
		                                                        AuthorityTokens.Study.Open
		                                                    }),

		                   new AuthorityGroupDefinition(DefaultAuthorityGroups.RadiologyResidents,
		                                                DefaultAuthorityGroups.RadiologyResidents,
		                                                false,
		                                                new string[]
		                                                    {
		                                                        AuthorityTokens.ViewerVisible,
		                                                        AuthorityTokens.Study.Open
		                                                    }),

		                   new AuthorityGroupDefinition(DefaultAuthorityGroups.EmergencyPhysicians,
		                                                DefaultAuthorityGroups.EmergencyPhysicians,
		                                                false,
		                                                new string[]
		                                                    {
		                                                        AuthorityTokens.ViewerVisible,
		                                                        AuthorityTokens.Study.Open
		                                                    })
		               };
		}

		#endregion
	}

	/// <summary>
	/// Common viewer authority tokens.
	/// </summary>
	public class AuthorityTokens
	{
		/// <summary>
		/// Permission required in order to see any viewer components (e.g. without this, all viewer components are hidden).
		/// </summary>
		[AuthorityToken(Description = "Permission required in order to see any viewer components (e.g. without this, all viewer components are hidden).")]
		public const string ViewerVisible = "Viewer/Visible";

		/// <summary>
		/// Study tokens.
		/// </summary>
		public class Study
		{
			/// <summary>
			/// Permission to open a study in the viewer.
			/// </summary>
			[AuthorityToken(Description = "Permission to open a study in the viewer.")]
			public const string Open = "Viewer/Study/Open";
		}
	}
}
