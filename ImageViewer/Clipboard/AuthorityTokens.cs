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

namespace ClearCanvas.ImageViewer.Clipboard
{
	[ExtensionOf(typeof(DefineAuthorityGroupsExtensionPoint), Enabled = false)]
	internal class DefineAuthorityGroups : IDefineAuthorityGroups
	{
		#region IDefineAuthorityGroups Members

		public AuthorityGroupDefinition[] GetAuthorityGroups()
		{
			return new[]
			{
				new AuthorityGroupDefinition(DefaultAuthorityGroups.HealthcareAdministrators,
					new []
					{
						AuthorityTokens.Clipboard.Export.JPG,
						AuthorityTokens.Clipboard.Export.AVI
					}),

				new AuthorityGroupDefinition(DefaultAuthorityGroups.Technologists,
					new []
					{
						AuthorityTokens.Clipboard.Export.JPG,
						AuthorityTokens.Clipboard.Export.AVI
					}),

				new AuthorityGroupDefinition(DefaultAuthorityGroups.Radiologists,
					new []
					{
						AuthorityTokens.Clipboard.Export.JPG,
						AuthorityTokens.Clipboard.Export.AVI
					}),

				new AuthorityGroupDefinition(DefaultAuthorityGroups.RadiologyResidents,
					new []
					{
						AuthorityTokens.Clipboard.Export.JPG,
						AuthorityTokens.Clipboard.Export.AVI
					}),

				new AuthorityGroupDefinition(DefaultAuthorityGroups.EmergencyPhysicians,
					new []
					{
						AuthorityTokens.Clipboard.Export.JPG,
						AuthorityTokens.Clipboard.Export.AVI
					})
			};
		}

		#endregion
	}

	/// <summary>
	/// Clipboard authority tokens.
	/// </summary>
	public class AuthorityTokens
	{
		/// <summary>
		/// Clipboard tokens
		/// </summary>
		public class Clipboard
		{
			/// <summary>
			/// Clipboard export tokens
			/// </summary>
			public class Export
			{
				/// <summary>
				/// Permission to export clipboard items into JPG files.
				/// </summary>
				[AuthorityToken(Description = "Permission to export clipboard items into JPG files.")]
				public const string JPG = "Viewer/Clipboard/Export/JPG";

				/// <summary>
				/// Permission to export clipboard items into AVI files.
				/// </summary>
				[AuthorityToken(Description = "Permission to export clipboard items into AVI files.")]
				public const string AVI = "Viewer/Clipboard/Export/AVI";
			}
		}
	}
}
