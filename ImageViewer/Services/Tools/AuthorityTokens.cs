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

namespace ClearCanvas.ImageViewer.Services.Tools
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
						AuthorityTokens.Administration.DicomServer,
						AuthorityTokens.Administration.DiskspaceManager,
						AuthorityTokens.Administration.Services,
						AuthorityTokens.Administration.ReindexLocalDataStore
                    })
            };
		}

		#endregion
	}

	public static class AuthorityTokens
	{
		public static class Administration
		{
			[AuthorityToken(Description = "Allow administration/configuration of the local DICOM Server (e.g. set AE Title, Port).")]
			public const string DicomServer = "Viewer/Administration/DICOM Server";

			[AuthorityToken(Description = "Allow configuration of disk space management.")]
			public const string DiskspaceManager = "Viewer/Administration/Diskspace Manager";

			[AuthorityToken(Description = "Permission to reindex the local data store.")]
			public const string ReindexLocalDataStore = "Viewer/Administration/Reindex Local Data Store";

			[AuthorityToken(Description = "Allow administration of the viewer services (e.g. Start/Stop/Restart).")]
			public const string Services = "Viewer/Administration/Services";
		}
	}
}
