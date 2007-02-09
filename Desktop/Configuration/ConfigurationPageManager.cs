using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using System.Collections;

namespace ClearCanvas.Desktop.Configuration
{
	[ExtensionPoint()]
	public class ConfigurationPageProviderExtensionPoint : ExtensionPoint<IConfigurationPageProvider>
	{
	}

	internal class ConfigurationPageManager : BasicExtensionPointManager<IConfigurationPageProvider>
	{
		private List<IConfigurationPage> _pageList;

		public ConfigurationPageManager()
		{
			base.LoadExtensions();
		}

		public IEnumerable<IConfigurationPageProvider> Providers
		{
			get{ return base.Extensions.AsReadOnly(); }
		}

		public IEnumerable<IConfigurationPage> Pages
		{ 
			get 
			{
				if (_pageList == null)
				{
					_pageList = new List<IConfigurationPage>();
					foreach (IConfigurationPageProvider provider in Providers)
						_pageList.AddRange(provider.GetPages());
				}

				return _pageList.AsReadOnly();
			}
		}

		protected override IExtensionPoint GetExtensionPoint()
		{
			return new ConfigurationPageProviderExtensionPoint();
		}
	}
}
