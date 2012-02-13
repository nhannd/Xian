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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Authorization;

namespace ClearCanvas.ImageViewer.Services
{
	internal class DefaultAuthorityGroups
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
						AuthorityTokens.Study.Delete,
						AuthorityTokens.Study.Retrieve,
						AuthorityTokens.Study.Send,
						AuthorityTokens.Study.Import
                    }),

				new AuthorityGroupDefinition(DefaultAuthorityGroups.Technologists,
                    DefaultAuthorityGroups.Technologists,
                    false,
                    new string[] 
                    {
						AuthorityTokens.Study.Delete,
						AuthorityTokens.Study.Retrieve,
						AuthorityTokens.Study.Send,
						AuthorityTokens.Study.Import
                    }),

                new AuthorityGroupDefinition(DefaultAuthorityGroups.Radiologists,
                    DefaultAuthorityGroups.Radiologists,
                    false,
                    new string[] 
                    {
						AuthorityTokens.Study.Delete,
						AuthorityTokens.Study.Retrieve,
						AuthorityTokens.Study.Send,
						AuthorityTokens.Study.Import
                   }),

                new AuthorityGroupDefinition(DefaultAuthorityGroups.RadiologyResidents,
                    DefaultAuthorityGroups.RadiologyResidents,
                    false,
                    new string[] 
                    {
						AuthorityTokens.Study.Delete,
						AuthorityTokens.Study.Retrieve,
						AuthorityTokens.Study.Send,
						AuthorityTokens.Study.Import
                   }),

                new AuthorityGroupDefinition(DefaultAuthorityGroups.EmergencyPhysicians,
                    DefaultAuthorityGroups.EmergencyPhysicians,
                    false,
                    new string[] 
                    {
						AuthorityTokens.Study.Delete,
						AuthorityTokens.Study.Retrieve
                    }),
            };
		}

		#endregion
	}
	
	public static class AuthorityTokens
	{
		public class Study
		{
			[AuthorityToken(Description = "Permission to send a study to another DICOM device (e.g. another workstation or PACS).")]
			public const string Send = "Viewer/Study/Send";

			[AuthorityToken(Description = "Permission to delete a study from the local store.")]
			public const string Delete = "Viewer/Study/Delete";

			[AuthorityToken(Description = "Permission to retrieve a study to the local store.")]
			public const string Retrieve = "Viewer/Study/Retrieve";

			[AuthorityToken(Description = "Permission to import study data into the local store.")]
			public const string Import = "Viewer/Study/Import";
		}
	}
}
