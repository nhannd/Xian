#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ExtensionOf(typeof(ConfigurationPageProviderExtensionPoint))]
	public class DicomExplorerConfigurationPageProvider : IConfigurationPageProvider
	{
		public DicomExplorerConfigurationPageProvider()
		{

		}

		#region IConfigurationPageProvider Members

		public IEnumerable<IConfigurationPage> GetPages()
		{
			List<IConfigurationPage> listPages = new List<IConfigurationPage>();

			if (PermissionsHelper.IsInRole(AuthorityTokens.DicomExplorer))
				listPages.Add(new ConfigurationPage<DicomExplorerConfigurationComponent>("PathExplorer/PathDicom"));

			return listPages.AsReadOnly();
		}

		#endregion
	}
}
