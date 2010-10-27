#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Configuration
{
	/// <summary>
	/// An extension point for <see cref="IConfigurationPageProvider"/>s.
	/// </summary>
	[ExtensionPoint()]
	public sealed class ConfigurationPageProviderExtensionPoint : ExtensionPoint<IConfigurationPageProvider>
	{
	}

	internal class ConfigurationPageManager
	{
		private IList<IConfigurationPage> _pageList;

		public ConfigurationPageManager()
		{
		}

		public IEnumerable<IConfigurationPage> Pages
		{ 
			get 
			{
				if (_pageList == null)
				{
					var list = new List<IConfigurationPage>();
					CollectionUtils.ForEach(new ConfigurationPageProviderExtensionPoint().CreateExtensions(),
					                        (IConfigurationPageProvider provider) => list.AddRange(provider.GetPages()));
					_pageList = list.AsReadOnly();
				}

				return _pageList;
			}
		}
	}
}
