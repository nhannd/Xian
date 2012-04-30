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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Configuration
{
	[ExtensionOf(typeof(ConfigurationPageProviderExtensionPoint))]
	[ExtensionOf(typeof(ActivityMonitorQuickLinkHandlerExtensionPoint))]
	public class ConfigurationPageProvider : IConfigurationPageProvider, IActivityMonitorQuickLinkHandler
	{
		private const string ServerConfigurationPath = "DicomConfiguration/ServerConfiguration";
		private const string StorageConfigurationPath = "DicomConfiguration/StorageConfiguration";

		#region IConfigurationPageProvider Members

		IEnumerable<IConfigurationPage> IConfigurationPageProvider.GetPages()
		{
			var listPages = new List<IConfigurationPage>();

			if (PermissionsHelper.IsInRole(Services.AuthorityTokens.Administration.DicomServer) && Common.DicomServer.DicomServer.IsSupported)
				listPages.Add(new ConfigurationPage<DicomServerConfigurationComponent>(ServerConfigurationPath));

            if (PermissionsHelper.IsInRole(Services.AuthorityTokens.Administration.Storage) && Common.StudyManagement.StudyStore.IsSupported)
				listPages.Add(new ConfigurationPage<StorageConfigurationComponent>(StorageConfigurationPath));
            
            return listPages.AsReadOnly();
		}

		#endregion

		bool IActivityMonitorQuickLinkHandler.CanHandle(ActivityMonitorQuickLink link)
		{
			return link == ActivityMonitorQuickLink.LocalServerConfiguration;
		}

		void IActivityMonitorQuickLinkHandler.Handle(ActivityMonitorQuickLink link, IDesktopWindow window)
		{
			if (link == ActivityMonitorQuickLink.LocalServerConfiguration)
			{
				ConfigurationDialog.Show(window, ServerConfigurationPath);
			}
		}
	}
}
