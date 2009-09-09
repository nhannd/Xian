using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Configuration;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Configuration
{
	[ExtensionOf(typeof (ConfigurationPageProviderExtensionPoint))]
	public class ConfigurationPageProvider : IConfigurationPageProvider
	{
		#region IConfigurationPageProvider Members

		public IEnumerable<IConfigurationPage> GetPages()
		{
			yield return new ConfigurationPage(MprConfigurationComponent.Path, new MprConfigurationComponent());
		}

		#endregion
	}
}