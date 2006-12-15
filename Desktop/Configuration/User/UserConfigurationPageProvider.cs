using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Configuration.User
{
	[ExtensionOf(typeof(ConfigurationPageProviderExtensionPoint))]
	public class UserConfigurationPageProvider : IConfigurationPageProvider
	{
		#region IConfigurationPageProvider Members

		public IConfigurationPage[] GetPages()
		{
			List<IConfigurationPage> listPages = new List<IConfigurationPage>();

			listPages.Add(new DateFormatConfigurationPage("TitleDateFormat"));
		
			return listPages.ToArray();
		}

		#endregion
	}
}
