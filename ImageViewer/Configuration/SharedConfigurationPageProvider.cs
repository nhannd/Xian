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
using ClearCanvas.ImageViewer.Common.ServerDirectory;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Configuration
{
	[ExtensionOf(typeof(SharedConfigurationPageProviderExtensionPoint))]
	[ExtensionOf(typeof(ActivityMonitorQuickLinkHandlerExtensionPoint))]
	public class SharedConfigurationPageProvider : IConfigurationPageProvider, IActivityMonitorQuickLinkHandler
	{
        public const string LocalConfigurationPath = "LocalConfiguration";
        public const string ServerConfigurationPath = LocalConfigurationPath + @"/" + "ServerConfiguration";
        public const string StorageConfigurationPath = LocalConfigurationPath + @"/" + "StorageConfiguration";
        public const string PriorsServerConfigurationPath = LocalConfigurationPath + @"/" + "PriorsServersConfiguration";
        public const string PublishingConfigurationPath = "PublishingConfiguration";

		#region IConfigurationPageProvider Members

		IEnumerable<IConfigurationPage> IConfigurationPageProvider.GetPages()
		{
			var listPages = new List<IConfigurationPage>();

			if (PermissionsHelper.IsInRole(Services.AuthorityTokens.Administration.DicomServer) && Common.DicomServer.DicomServer.IsSupported)
				listPages.Add(new ConfigurationPage<DicomServerConfigurationComponent>(ServerConfigurationPath));

            if (PermissionsHelper.IsInRole(Services.AuthorityTokens.Administration.Storage) && Common.StudyManagement.StudyStore.IsSupported)
                listPages.Add(new ConfigurationPage<StorageConfigurationComponent>(StorageConfigurationPath));

            if (PermissionsHelper.IsInRoles(AuthorityTokens.Configuration.PriorsServers) && ServerDirectory.IsSupported)
                listPages.Add(new ConfigurationPage<PriorsServersConfigurationComponent>(PriorsServerConfigurationPath));

            if (PermissionsHelper.IsInRole(AuthorityTokens.Configuration.Publishing))
                listPages.Add(new ConfigurationPage(PublishingConfigurationPath, new PublishingConfigurationComponent()));

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
				SharedConfigurationDialog.Show(window, ServerConfigurationPath);
			}
		}
	}
}
