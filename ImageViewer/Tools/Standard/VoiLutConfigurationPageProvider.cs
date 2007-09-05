using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[ExtensionOf(typeof(ConfigurationPageProviderExtensionPoint))]
	public class VoiLutConfigurationPageProvider : IConfigurationPageProvider
	{
		public VoiLutConfigurationPageProvider()
		{

		}

		#region IConfigurationPageProvider Members

		public IEnumerable<IConfigurationPage> GetPages()
		{
			List<IConfigurationPage> listPages = new List<IConfigurationPage>();

			listPages.Add(new ConfigurationPage<VoiLutConfigurationComponent>("Window & Level"));

			return listPages.AsReadOnly();
		}

		#endregion
	}
}
