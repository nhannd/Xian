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

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Authorization;
using ClearCanvas.Desktop.Explorer;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer.Explorer.Local
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
						AuthorityTokens.MyComputer
				   }),

                new AuthorityGroupDefinition(DefaultAuthorityGroups.Technologists,
                    new string[] 
                    {
						AuthorityTokens.MyComputer
					}),

                new AuthorityGroupDefinition(DefaultAuthorityGroups.Radiologists,
                    new string[] 
                    {
						AuthorityTokens.MyComputer
					}),

                new AuthorityGroupDefinition(DefaultAuthorityGroups.RadiologyResidents,
                    new string[] 
                    {
						AuthorityTokens.MyComputer
					})
            };
		}

		#endregion
	}
	
	public static class AuthorityTokens
	{
		[AuthorityToken(Description = "Grant access to the 'My Computer' explorer.")]
		public const string MyComputer = "Viewer/Explorer/My Computer";
	}

	[ExtensionOf(typeof(HealthcareArtifactExplorerExtensionPoint))]
	public class LocalImageExplorer : IHealthcareArtifactExplorer
	{
		LocalImageExplorerComponent _component;

		public LocalImageExplorer()
		{

		}

		#region IHealthcareArtifactExplorer Members

		public string Name
		{
			get { return SR.MyComputer; }
		}

		public bool IsAvailable
		{
			get { return PermissionsHelper.IsInRole(AuthorityTokens.MyComputer); }
		}

		public IApplicationComponent Component
		{
			get
			{
				if (_component == null && IsAvailable)
					_component = new LocalImageExplorerComponent();

				return _component;
			}
		}

		#endregion

	}
}
