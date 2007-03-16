using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Configuration
{
	[ExtensionOf(typeof(ConfigurationPageProviderExtensionPoint))]
	public class DicomServerConfigurationPageProvider : IConfigurationPageProvider
    {
        public DicomServerConfigurationPageProvider()
		{

		}

		#region IConfigurationPageProvider Members

		public IEnumerable<IConfigurationPage> GetPages()
		{
			List<IConfigurationPage> listPages = new List<IConfigurationPage>();

            listPages.Add(new ConfigurationPage<DicomServerConfigurationComponent>(SR.DicomServerConfiguration));

			return listPages.AsReadOnly();
		}

		#endregion
	}
}
