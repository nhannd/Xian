using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Configuration
{
	[ExtensionOf(typeof(ConfigurationPageProviderExtensionPoint))]
	public class DiskspaceManagerConfigurationPageProvider : IConfigurationPageProvider
    {
        public DiskspaceManagerConfigurationPageProvider()
		{

		}

		#region IConfigurationPageProvider Members

		public IEnumerable<IConfigurationPage> GetPages()
		{
			List<IConfigurationPage> listPages = new List<IConfigurationPage>();

            listPages.Add(new ConfigurationPage<DiskspaceManagerConfigurationComponent>(SR.DiskspaceManagerConfiguration));

			return listPages.AsReadOnly();
		}

		#endregion
	}
}
