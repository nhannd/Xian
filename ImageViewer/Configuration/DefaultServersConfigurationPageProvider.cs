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
	public class DefaultServersConfigurationPageProvider : IConfigurationPageProvider
	{
		public DefaultServersConfigurationPageProvider()
		{
		}

		#region IConfigurationPageProvider Members

		public IEnumerable<IConfigurationPage> GetPages()
		{
			List<IConfigurationPage> listPages = new List<IConfigurationPage>();

			if (PermissionsHelper.IsInRoles(AuthorityTokens.Configuration.DefaultServers))
				listPages.Add(new ConfigurationPage<DefaultServersConfigurationComponent>("DicomConfiguration/DefaultServerConfiguration"));
			
			return listPages.AsReadOnly();
		}

		#endregion
	}
}
