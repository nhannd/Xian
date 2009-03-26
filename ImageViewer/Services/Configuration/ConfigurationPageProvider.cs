using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Configuration;
using System.Threading;
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer.Services.Configuration
{
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

			if (PermissionsHelper.IsInRole(ImageViewer.Services.AuthorityTokens.Admin.System.DicomServer))
				listPages.Add(new ConfigurationPage<DicomServerConfigurationComponent>("DicomServerConfiguration"));

			if (PermissionsHelper.IsInRole(ImageViewer.Services.AuthorityTokens.Admin.System.DiskspaceManagement))
				listPages.Add(new ConfigurationPage<DiskspaceManagerConfigurationComponent>("DiskspaceManagerConfiguration"));

			listPages.Add(new ConfigurationPage("DefaultServerConfiguration", new DefaultServersConfigurationComponent()));

			return listPages.AsReadOnly();
		}

		#endregion
	}
}
