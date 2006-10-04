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

	public class ConfigurationPageManager : BasicExtensionPointManager<IConfigurationPageProvider>
	{
		private class SortPagesByPath : IComparer<IConfigurationPage>
		{
			#region IComparer<IConfigurationPage> Members

			public int Compare(IConfigurationPage x, IConfigurationPage y)
			{
				if (x == null)
				{
					if (y == null)
						return 0;
					else
						return -1;
				}

				if (y == null)
					return 1;

				return x.GetPath().CompareTo(y.GetPath());
			}

			#endregion
		}

		List<IConfigurationPage> _pageList;

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
				if (_pageList != null)
					return _pageList;

				_pageList = new List<IConfigurationPage>();
				foreach (IConfigurationPageProvider provider in Providers)
				{
					foreach (IConfigurationPage configurationPage in provider.GetPages())
						_pageList.Add(configurationPage);
				}

				_pageList.Sort((IComparer<IConfigurationPage>)new SortPagesByPath());
				return _pageList;
			}
		}

		protected override IExtensionPoint GetExtensionPoint()
		{
			return new ConfigurationPageProviderExtensionPoint();
		}
	}
}
