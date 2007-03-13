using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[ExtensionOf(typeof(ConfigurationPageProviderExtensionPoint))]
	public class WindowLevelConfigurationPageProvider : IConfigurationPageProvider
	{
		public WindowLevelConfigurationPageProvider()
		{

		}

		#region IConfigurationPageProvider Members

		public IEnumerable<IConfigurationPage> GetPages()
		{
			List<IConfigurationPage> listPages = new List<IConfigurationPage>();

			listPages.Add(new ConfigurationPage<WindowLevelConfigurationApplicationComponent>(SR.WindowLevelConfiguration));

			return listPages.AsReadOnly();
		}

		#endregion
	}
}
