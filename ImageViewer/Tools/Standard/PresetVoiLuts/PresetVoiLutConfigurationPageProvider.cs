using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Configuration;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts
{
	[ExtensionOf(typeof(ConfigurationPageProviderExtensionPoint))]
	public class PresetVoiLutConfigurationPageProvider : IConfigurationPageProvider
	{
		public PresetVoiLutConfigurationPageProvider()
		{

		}

		#region IConfigurationPageProvider Members

		public IEnumerable<IConfigurationPage> GetPages()
		{
			List<IConfigurationPage> listPages = new List<IConfigurationPage>();

			listPages.Add(new ConfigurationPage<PresetVoiLutConfigurationComponent>("Window & Level"));

			return listPages.AsReadOnly();
		}

		#endregion
	}
}