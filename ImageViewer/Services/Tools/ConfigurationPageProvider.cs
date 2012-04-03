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

namespace ClearCanvas.ImageViewer.Services.Tools
{
    //TODO (Marmot):Could all the stuff in this assembly get moved to ImageViewer?

	[ExtensionOf(typeof(ConfigurationPageProviderExtensionPoint))]
	public class ConfigurationPageProvider : IConfigurationPageProvider
	{
		public ConfigurationPageProvider()
		{
		}

		#region IConfigurationPageProvider Members

		public IEnumerable<IConfigurationPage> GetPages()
		{
			List<IConfigurationPage> listPages = new List<IConfigurationPage>();

			if (PermissionsHelper.IsInRole(AuthorityTokens.Administration.DicomServer))
				listPages.Add(new ConfigurationPage<DicomServerConfigurationComponent>("DicomConfiguration/ServerConfiguration"));

		    //TODO (Marmot): Restore, or refactor.
            //if (PermissionsHelper.IsInRole(AuthorityTokens.Administration.DiskspaceManager))
            //    listPages.Add(new ConfigurationPage<DiskspaceManagerConfigurationComponent>("DiskspaceManagerConfiguration"));

			return listPages.AsReadOnly();
		}

		#endregion
	}
}
