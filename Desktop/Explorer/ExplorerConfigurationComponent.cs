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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;

namespace ClearCanvas.Desktop.Explorer
{
	[ExtensionOf(typeof(ConfigurationPageProviderExtensionPoint))]
	internal class ConfigurationPageProvider : IConfigurationPageProvider
	{
		#region IConfigurationPageProvider Members

		public IEnumerable<IConfigurationPage> GetPages()
		{
			if (!ExplorerLocalSettings.Default.ExplorerIsPrimary && ExplorerTool.GetExplorers().Count > 0)
				yield return new ConfigurationPage<ExplorerConfigurationComponent>("PathExplorer");
		}

		#endregion
	}

	[ExtensionPoint]
	public sealed class ExplorerConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(ExplorerConfigurationComponentViewExtensionPoint))]
	public class ExplorerConfigurationComponent : ConfigurationApplicationComponent
	{
		private bool _launchAsShelf;
		private bool _launchAtStartup;

		public ExplorerConfigurationComponent()
		{
			_launchAsShelf = ExplorerSettings.Default.LaunchAsShelf;
			_launchAtStartup = ExplorerSettings.Default.LaunchAtStartup;
		}

		#region Presentation Model

		public new bool LaunchAsWorkspace
		{
			get { return !_launchAsShelf; }
			set
			{
				if (value)
					LaunchAsShelf = false;
				else
					LaunchAsShelf = true;
			}
		}

		public new bool LaunchAsShelf
		{
			get { return _launchAsShelf; }
			set
			{
				if (_launchAsShelf != value)
				{
					_launchAsShelf = value;
					NotifyPropertyChanged("LaunchAsShelf");
					NotifyPropertyChanged("LaunchAsWorkspace");

					Modified = true;
				}
			}
		}

		public bool LaunchAtStartup
		{
			get { return _launchAtStartup; }
			set
			{
				if (_launchAtStartup != value)
				{
					_launchAtStartup = value;
					NotifyPropertyChanged("LaunchAtStartup");

					Modified = true;
				}
			}
		}

		#endregion

		public override void Save()
		{
			ExplorerSettings.Default.LaunchAsShelf = _launchAsShelf;
			ExplorerSettings.Default.LaunchAtStartup = _launchAtStartup;

			ExplorerSettings.Default.Save();
		}
	}
}
