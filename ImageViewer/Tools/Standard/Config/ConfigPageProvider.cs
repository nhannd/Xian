using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Configuration;

namespace ClearCanvas.ImageViewer.Tools.Standard.Config
{
	[ExtensionOf(typeof (ConfigurationPageProviderExtensionPoint))]
	public class ConfigPageProvider : IConfigurationPageProvider
	{
		public IEnumerable<IConfigurationPage> GetPages()
		{
			List<IConfigurationPage> list = new List<IConfigurationPage>(1);
			list.Add(new ConfigurationPage<StandardToolsConfigComponent>("StandardTools"));
			return list.AsReadOnly();
		}
	}
}