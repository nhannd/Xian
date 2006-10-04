using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Configuration.Application
{
	[ExtensionOf(typeof(ConfigurationPageProviderExtensionPoint))]
	public class ApplicationConfigurationPageProvider : IConfigurationPageProvider
	{
		#region IConfigurationPageProvider Members

		public IConfigurationPage[] GetPages()
		{
			List<IConfigurationPage> listPages = new List<IConfigurationPage>();
			listPages.Add(new DateFormatApplicationConfigurationPage("DateFormat"));
			return listPages.ToArray();
		}

		#endregion
	}
}
