using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	[ExtensionOf(typeof(ConfigurationPageProviderExtensionPoint))]
	public class LayoutConfigurationPageProvider : IConfigurationPageProvider
	{
		public LayoutConfigurationPageProvider()
		{
		}

		#region IConfigurationPageProvider Members

		public IEnumerable<IConfigurationPage> GetPages()
		{
			List<IConfigurationPage> listPages = new List<IConfigurationPage>();

			listPages.Add(new ConfigurationPage<LayoutConfigurationApplicationComponent>("TitleLayoutConfiguration"));

			return listPages.AsReadOnly();
		}

		#endregion
	}
}
