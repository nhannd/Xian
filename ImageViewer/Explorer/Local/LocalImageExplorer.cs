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
using ClearCanvas.Desktop.Explorer;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Explorer.Local
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
