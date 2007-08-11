using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Validation;

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
				// We want to validate all configuration pages
				this.ValidationStrategy = new VisitedNodesContainerValidationStrategy();

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

			public override void Accept()
			{
				if (this.HasValidationErrors)
					ShowValidation(true);
				else
				{
					try
					{
						foreach (IConfigurationPage configurationPage in this.ConfigurationPages)
						{
							if (configurationPage.GetComponent().Modified)
								configurationPage.SaveConfiguration();
						}

						this.Host.Exit();
					}
					catch (Exception e)
					{
						ExceptionHandler.Report(e, SR.ExceptionFailedToSave, this.Host.DesktopWindow,
							delegate()
							{
								this.ExitCode = ApplicationComponentExitCode.Error;
								this.Host.Exit();
							});
					}
				}
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

			return exitCode;
		}
	}
}
