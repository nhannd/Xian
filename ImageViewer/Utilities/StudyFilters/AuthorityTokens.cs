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

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters
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
			       		                             		AuthorityTokens.StudyFilters,
			       		                             		AuthorityTokens.Study.Export,
			       		                             		AuthorityTokens.Study.Anonymize
			       		                             	}),
			       		new AuthorityGroupDefinition(DefaultAuthorityGroups.Radiologists,
			       		                             new string[]
			       		                             	{
			       		                             		AuthorityTokens.StudyFilters,
			       		                             		AuthorityTokens.Study.Export,
			       		                             		AuthorityTokens.Study.Anonymize
			       		                             	}),
			       		new AuthorityGroupDefinition(DefaultAuthorityGroups.RadiologyResidents,
			       		                             new string[]
			       		                             	{
			       		                             		AuthorityTokens.StudyFilters,
			       		                             		AuthorityTokens.Study.Export,
			       		                             		AuthorityTokens.Study.Anonymize
			       		                             	}),
			       		new AuthorityGroupDefinition(DefaultAuthorityGroups.RadiologyResidents,
			       		                             new string[]
			       		                             	{
			       		                             		AuthorityTokens.StudyFilters,
			       		                             		AuthorityTokens.Study.Export,
			       		                             		AuthorityTokens.Study.Anonymize
			       		                             	})
			       	};
		}

		#endregion
	}

	public static class AuthorityTokens
	{
		[AuthorityToken(Description = "Grant access to the Study Filters.")]
		public const string StudyFilters = "Viewer/Study Filters";

		public static class Study
		{
			[AuthorityToken(Description = "Permission to copy an unanonymized study out of the viewer.")]
			public const string Export = "Viewer/Study/Export ";

			[AuthorityToken(Description = "Permission to anonymize a study in the viewer.")]
			public const string Anonymize = "Viewer/Study/Anonymize";
		}
	}
}