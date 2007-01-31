using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop.Configuration.Tools
{
	[MenuAction("show", "global-menus/MenuTools/MenuOptions", KeyStroke = XKeys.Control | XKeys.O)]
	[Tooltip("show", "MenuOptions")]
	[IconSet("show", IconScheme.Colour, "Icons.MenuOptionsSmall.png", "Icons.MenuOptionsMedium.png", "Icons.MenuOptionsLarge.png")]
	[ClickHandler("show", "Show")]
	[EnabledStateObserver("show", "Enabled", "EnabledChanged")]
	[GroupHint("show", "Application.Options")]

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
				ConfigurationDialog configurationDialog = new ConfigurationDialog();
				configurationDialog.Show(this.Context.DesktopWindow);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}
	}
}
