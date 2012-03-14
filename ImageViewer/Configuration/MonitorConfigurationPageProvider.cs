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
	public class MonitorConfigurationPageProvider : IConfigurationPageProvider
	{
		public MonitorConfigurationPageProvider()
		{

		}

		#region IConfigurationPageProvider Members

		public IEnumerable<IConfigurationPage> GetPages()
		{
			List<IConfigurationPage> listPages = new List<IConfigurationPage>();
			if (PermissionsHelper.IsInRole(ImageViewer.AuthorityTokens.ViewerVisible))
				listPages.Add(new ConfigurationPage<MonitorConfigurationApplicationComponent>("MonitorConfiguration"));
			return listPages.AsReadOnly();
		}

		#endregion
	}
}