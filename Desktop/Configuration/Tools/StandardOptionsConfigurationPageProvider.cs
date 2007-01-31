using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.Desktop.Configuration.Standard;

namespace ClearCanvas.Desktop.Configuration.Tools
{
	[ExtensionOf(typeof(ConfigurationPageProviderExtensionPoint))]
	public class StandardOptionsConfigurationPageProvider : IConfigurationPageProvider
	{
		public StandardOptionsConfigurationPageProvider()
		{ 
		}

		#region IConfigurationPageProvider Members

		public IEnumerable<IConfigurationPage> GetPages()
		{
			List<IConfigurationPage> listPages = new List<IConfigurationPage>();

			listPages.Add(new DateFormatConfigurationPage("TitleDateFormat"));
		
			return listPages.AsReadOnly();
		}

		#endregion
	}
}
