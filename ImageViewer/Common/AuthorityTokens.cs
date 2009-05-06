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

namespace ClearCanvas.ImageViewer.Common
{
	[ExtensionOf(typeof(DefineAuthorityGroupsExtensionPoint))]
	public class DefaultAuthorityGroups : IDefineAuthorityGroups
	{
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
						AuthorityTokens.Workflow.Study.Search,
						AuthorityTokens.Workflow.Study.View,
						AuthorityTokens.Workflow.Study.Export,
						AuthorityTokens.Workflow.Study.Create,
						AuthorityTokens.Workflow.Study.Modify,
						AuthorityTokens.Workflow.Study.Delete
                    }),

				new AuthorityGroupDefinition(Clerical,
				    new string[] 
				    {
						AuthorityTokens.Workflow.Study.Search
				   }),

                new AuthorityGroupDefinition(Technologists,
                    new string[] 
                    {
						AuthorityTokens.Workflow.Study.Search,
						AuthorityTokens.Workflow.Study.View,
						AuthorityTokens.Workflow.Study.Export,
						AuthorityTokens.Workflow.Study.Create,
						AuthorityTokens.Workflow.Study.Modify,
						AuthorityTokens.Workflow.Study.Delete
                    }),

                new AuthorityGroupDefinition(Radiologists,
                    new string[] 
                    {
						AuthorityTokens.Workflow.Study.Search,
						AuthorityTokens.Workflow.Study.View,
						AuthorityTokens.Workflow.Study.Export,
						AuthorityTokens.Workflow.Study.Create,
						AuthorityTokens.Workflow.Study.Modify,
						AuthorityTokens.Workflow.Study.Delete
                   }),

                new AuthorityGroupDefinition(RadiologyResidents,
                    new string[] 
                    {
						AuthorityTokens.Workflow.Study.Search,
						AuthorityTokens.Workflow.Study.View,
						AuthorityTokens.Workflow.Study.Export,
						AuthorityTokens.Workflow.Study.Create,
						AuthorityTokens.Workflow.Study.Modify,
						AuthorityTokens.Workflow.Study.Delete
                   }),

                new AuthorityGroupDefinition(EmergencyPhysicians,
                    new string[] 
                    {
						AuthorityTokens.Workflow.Study.Search,
						AuthorityTokens.Workflow.Study.View,
						AuthorityTokens.Workflow.Study.Export
                    }),
            };
		}

		#endregion
	}

	public static class AuthorityTokens
	{
		public class Workflow
		{
			public class Study
			{
				[AuthorityToken(Description = "Generic user permission to search the study data in the viewer.")]
				public const string Search = "Viewer/Workflow/Study/Search";

				[AuthorityToken(Description = "Generic user permission to export study data from the viewer.")]
				public const string Export = "Viewer/Workflow/Study/Export";

				[AuthorityToken(Description = "Generic user permission to view study data in the viewer.")]
				public const string View = "Viewer/Workflow/Study/View";

				[AuthorityToken(Description = "Generic user permission to create a new study in the viewer.")]
				public const string Create = "Viewer/Workflow/Study/Create";

				[AuthorityToken(Description = "Generic user permission to modify study data in the viewer.")]
				public const string Modify = "Viewer/Workflow/Study/Modify";

				[AuthorityToken(Description = "Generic user permission to delete a study from the viewer.")]
				public const string Delete = "Viewer/Workflow/Study/Delete";
			}
		}
	}
}
