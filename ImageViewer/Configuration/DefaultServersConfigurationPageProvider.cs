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

			listPages.Add(new ConfigurationPage<DefaultServersConfigurationComponent>("DefaultServerConfiguration"));
			
			return listPages.AsReadOnly();
		}

		#endregion
	}
}
