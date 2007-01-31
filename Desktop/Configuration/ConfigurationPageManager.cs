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

					_pageList.Sort((IComparer<IConfigurationPage>)new SortPagesByPath());
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
