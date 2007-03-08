using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ExtensionOf(typeof(ConfigurationPageProviderExtensionPoint))]
	public class DicomExplorerConfigurationPageProvider : IConfigurationPageProvider
	{
		public DicomExplorerConfigurationPageProvider()
		{

		}

		#region IConfigurationPageProvider Members

		public IEnumerable<IConfigurationPage> GetPages()
		{
			List<IConfigurationPage> listPages = new List<IConfigurationPage>();

			listPages.Add(new ConfigurationPage<DicomExplorerConfigurationApplicationComponent>(SR.DicomExplorerConfiguration));

			return listPages.AsReadOnly();
		}

		#endregion
	}
}
