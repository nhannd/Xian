using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Configuration
{
	public class ConfigurationDialog
	{
		public ConfigurationDialog()
		{
		}

		private class ConfigurationNavigatorComponentContainer : NavigatorComponentContainer
		{
			private ConfigurationPageManager _configurationPageManager;

			public ConfigurationNavigatorComponentContainer()
				: base()
			{
				_configurationPageManager = new ConfigurationPageManager();

				foreach (IConfigurationPage configurationPage in this.ConfigurationPages)
					this.Pages.Add(new NavigatorPage(configurationPage.GetPath(), configurationPage.GetComponent()));

				if (Pages.Count > 0)
				{
					MoveTo(0);
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

		public ApplicationComponentExitCode Show(IDesktopWindow desktopWindow)
		{
			ConfigurationNavigatorComponentContainer container = new ConfigurationNavigatorComponentContainer();
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
