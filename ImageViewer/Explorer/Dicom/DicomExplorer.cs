#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Explorer;
using ClearCanvas.Common.Authorization;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	public static class AuthorityTokens
	{
		[AuthorityToken(Description = "Grant access to the DICOM explorer.")]
		public const string DicomExplorer = "Viewer/Explorer/DICOM";

		public static class Configuration
		{
			[AuthorityToken(Description = "Allow configuration of 'My Servers'.")]
			public const string MyServers = "Viewer/Configuration/My Servers";
		}
	}

	[ExtensionOf(typeof(HealthcareArtifactExplorerExtensionPoint))]
	public class DicomExplorer : IHealthcareArtifactExplorer
	{
		private DicomExplorerComponent _component;

		public DicomExplorer()
		{
		}

		#region IHealthcareArtifactExplorer Members

		public string Name
		{
			get { return SR.TitleDicomExplorer; }
		}


		public bool IsAvailable
		{
			get { return PermissionsHelper.IsInRole(AuthorityTokens.DicomExplorer); }
		}

		public IApplicationComponent Component
		{
			get
			{
				if (_component == null && IsAvailable)
					_component = DicomExplorerComponent.Create();

				return _component;
			}
		}

		#endregion
	}
}
