using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.Configuration
{
	[MenuAction("show", "global-menus/MenuTools/MenuOptions", KeyStroke = XKeys.Control | XKeys.O)]
	[Tooltip("show", "MenuOptions")]
	[IconSet("show", IconScheme.Colour, "Icons.MenuOptionsSmall.png", "Icons.MenuOptionsMedium.png", "Icons.MenuOptionsLarge.png")]
	[ClickHandler("show", "Show")]
	[EnabledStateObserver("show", "Enabled", "EnabledChanged")]

	[ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
	public class MenuOptions : Tool<ClearCanvas.Desktop.IDesktopToolContext>
	{
		private bool _enabled;
		private event EventHandler _enabledChanged;

		/// <summary>
		/// Default constructor.  A no-args constructor is required by the
		/// framework.  Do not remove.
		/// </summary>
		public MenuOptions()
		{
			_enabled = true;
		}

		/// <summary>
		/// Called by the framework to initialize this tool.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();

			// TODO: add any significant initialization code here rather than in the constructor
		}

		/// <summary>
		/// Called to determine whether this tool is enabled/disabled in the UI.
		/// </summary>
		public bool Enabled
		{
			get { return _enabled; }
			protected set
			{
				if (_enabled != value)
				{
					_enabled = value;
					EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Notifies that the Enabled state of this tool has changed.
		/// </summary>
		public event EventHandler EnabledChanged
		{
			add { _enabledChanged += value; }
			remove { _enabledChanged -= value; }
		}

		/// <summary>
		/// Called by the framework when the user clicks the "Options" menu item or toolbar button.
		/// </summary>
		public void Show()
		{
			try
			{
				ConfigurationPageManager configurationPageManager = new ConfigurationPageManager();

				IEnumerable<IConfigurationPage> pages = configurationPageManager.Pages;

				NavigatorComponentContainer container = new NavigatorComponentContainer();

				foreach (IConfigurationPage configurationPage in pages)
					container.Pages.Add(new NavigatorPage(configurationPage.GetPath(), configurationPage.GetComponent()));

				if (container.Pages.Count > 0)
				{
					try
					{
						container.CurrentPage = container.Pages[0];
						if (ApplicationComponentExitCode.Normal == ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, container, SR.TitleMenuOptions))
						{
							foreach (IConfigurationPage configurationPage in pages)
							{
								if (configurationPage.GetComponent().Modified)
									configurationPage.SaveConfiguration();
							}
						}
					}
					catch (Exception e)
					{
						Platform.Log(e);
					}
					finally
					{
						container.Stop();
					}
				}
				else
				{
					Platform.ShowMessageBox(SR.MessageNoConfigurationPagesExist);
				}
			}
			catch (Exception e)
			{
				Platform.Log(e);
				Platform.ShowMessageBox(e.Message);
			}
		}
	}
}
