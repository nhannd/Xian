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
						AuthorityTokens.ViewerVisible,
						AuthorityTokens.Study.Open
				   }),

				new AuthorityGroupDefinition(DefaultAuthorityGroups.Clerical,
				    new string[] 
				    {
						AuthorityTokens.ViewerVisible
				   }),

                new AuthorityGroupDefinition(DefaultAuthorityGroups.Technologists,
                    new string[] 
                    {
						AuthorityTokens.ViewerVisible,
						AuthorityTokens.Study.Open
                    }),

                new AuthorityGroupDefinition(DefaultAuthorityGroups.Radiologists,
                    new string[] 
                    {
						AuthorityTokens.ViewerVisible,
						AuthorityTokens.Study.Open
                   }),

                new AuthorityGroupDefinition(DefaultAuthorityGroups.RadiologyResidents,
                    new string[] 
                    {
						AuthorityTokens.ViewerVisible,
						AuthorityTokens.Study.Open
                   }),

                new AuthorityGroupDefinition(DefaultAuthorityGroups.EmergencyPhysicians,
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
