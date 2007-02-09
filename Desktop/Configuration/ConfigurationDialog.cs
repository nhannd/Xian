using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Configuration
{
	public sealed class ConfigurationDialog
	{
		private ConfigurationDialog()
		{
		}

		private class ConfigurationNavigatorComponentContainer : NavigatorComponentContainer
		{
			private ConfigurationPageManager _configurationPageManager;

			public ConfigurationNavigatorComponentContainer(string initialPagePath)
				: base()
			{
				_configurationPageManager = new ConfigurationPageManager();

				List<NavigatorPage> pages = new List<NavigatorPage>();

				foreach (IConfigurationPage configurationPage in this.ConfigurationPages)
					pages.Add(new NavigatorPage(configurationPage.GetPath(), configurationPage.GetComponent()));

				pages.Sort(new NavigatorPageSortByPath());

				int initialPage = 0;
				int i = 0;

				foreach (NavigatorPage page in pages)
				{
					//do the unresolved paths match?
					if (page.Path.ToString() == initialPagePath)
						initialPage = i;

					this.Pages.Add(page);
					++i;
				}

				if (Pages.Count > 0)
				{
					MoveTo(initialPage);
				}
				else
				{
					throw new Exception(SR.MessageNoConfigurationPagesExist);
				}
			}

			public IEnumerable<IConfigurationPage> ConfigurationPages
			{
				get { return _configurationPageManager.Pages; }
			}
		}

		public static ApplicationComponentExitCode Show(IDesktopWindow desktopWindow)
		{
			return ConfigurationDialog.Show(desktopWindow, null);
		}

		public static ApplicationComponentExitCode Show(IDesktopWindow desktopWindow, string initialPageIdentifier)
		{
			ConfigurationNavigatorComponentContainer container = new ConfigurationNavigatorComponentContainer(initialPageIdentifier);
			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(desktopWindow, container, SR.TitleMenuOptions);
			if (ApplicationComponentExitCode.Normal == exitCode)
			{
				foreach (IConfigurationPage configurationPage in container.ConfigurationPages)
				{
					if (configurationPage.GetComponent().Modified)
						configurationPage.SaveConfiguration();
				}
			}

			return exitCode;
		}
	}
}
