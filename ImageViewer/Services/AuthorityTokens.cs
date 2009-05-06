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

namespace ClearCanvas.ImageViewer.Services
{
	[ExtensionOf(typeof(DefineAuthorityGroupsExtensionPoint))]
	internal class DefaultAuthorityGroups : IDefineAuthorityGroups
	{
		//TODO: make these only exist in ImageViewer.Common once desktop refactoring is done and this assembly essentially moves to Common
		public const string Administrators = "Administrators";
		public const string HealthcareAdministrators = "Healthcare Administrators";
		public const string Clerical = "Clerical";
		public const string Technologists = "Technologists";
		public const string Radiologists = "Radiologists";
		public const string RadiologyResidents = "Radiology Residents";
		public const string EmergencyPhysicians = "Emergency Physicians";

		#region IDefineAuthorityGroups Members

		public AuthorityGroupDefinition[] GetAuthorityGroups()
		{
			return new AuthorityGroupDefinition[]
            {
                new AuthorityGroupDefinition(Administrators,
                    new string[]
                    {
						AuthorityTokens.Admin.System.DiskspaceManagement,
						AuthorityTokens.Admin.System.DataStore,
						AuthorityTokens.Admin.System.DicomServer,
						AuthorityTokens.Management.Services,
						AuthorityTokens.Management.DataStore,
						AuthorityTokens.Management.DicomServer
                    })
			};
		}

		#endregion
	}

	public class AuthorityTokens
	{
		public class Admin
		{
			public class System
			{
				[AuthorityToken(Description = "Allows administrative configuration of Diskspace Management.")]
				public const string DiskspaceManagement = "Viewer/Admin/System/Diskspace Management";

				[AuthorityToken(Description = "Allows administrative configuration of the viewer Data Store.")]
				public const string DataStore = "Viewer/Admin/System/Data Store";

				[AuthorityToken(Description = "Allows administrative configuration of the viewer DICOM Server.")]
				public const string DicomServer = "Viewer/Admin/System/DICOM Server";
			}
		}

		public class Management
		{
			[AuthorityToken(Description = "Allow management of the viewer services (e.g restarting shred host).")]
			public const string Services = "Viewer/Management/Services";

			[AuthorityToken(Description = "Allow management of the Data Store (e.g. Reindexing).")]
			public const string DataStore = "Viewer/Management/Data Store";

			[AuthorityToken(Description = "Allow management of the viewer DICOM Server.")]
			public const string DicomServer = "Viewer/Management/DICOM Server";
		}
	}
}
