#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Configuration;

namespace ClearCanvas.ImageViewer.Configuration
{
	[ExtensionOf(typeof(ConfigurationPageProviderExtensionPoint))]
	public class ConfigurationPageProvider : IConfigurationPageProvider
	{
		#region IConfigurationPageProvider Members

		public IEnumerable<IConfigurationPage> GetPages()
		{
			var listPages = new List<IConfigurationPage>();

			if (PermissionsHelper.IsInRole(Services.AuthorityTokens.Administration.DicomServer) && Common.DicomServer.DicomServer.IsSupported)
				listPages.Add(new ConfigurationPage<DicomServerConfigurationComponent>("DicomConfiguration/ServerConfiguration"));

            if (PermissionsHelper.IsInRole(Services.AuthorityTokens.Administration.Storage) && Common.StudyManagement.StudyStore.IsSupported)
                listPages.Add(new ConfigurationPage<StorageConfigurationComponent>("DicomConfiguration/StorageConfiguration"));
            
            return listPages.AsReadOnly();
		}

		#endregion
	}
}
