#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using ClearCanvas.Common;
using ClearCanvas.Common.Authorization;

namespace ClearCanvas.ImageViewer.Services.Tools
{
	[ExtensionOf(typeof(DefineAuthorityGroupsExtensionPoint))]
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
