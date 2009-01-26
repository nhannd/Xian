using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Configuration;

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

			listPages.Add(new ConfigurationPage<DicomServerConfigurationComponent>("DicomServerConfiguration"));
			listPages.Add(new ConfigurationPage<DiskspaceManagerConfigurationComponent>("DiskspaceManagerConfiguration"));
			listPages.Add(new ConfigurationPage<DefaultServersConfigurationComponent>("DefaultServerConfiguration"));

			return listPages.AsReadOnly();
		}

		#endregion
	}
}
