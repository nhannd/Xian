using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Samples.WebBrowser
{
	[MenuAction("apply", "global-menus/MenuTools/MenuStandard/MenuWebBrowser")]
	[ButtonAction("apply", "global-toolbars/ToolbarStandard/ToolbarWebBrowser")]
	[Tooltip("apply", "TooltipWebBrowser")]
	[IconSet("apply", IconScheme.Colour, "Icons.WebBrowserToolSmall.png", "Icons.WebBrowserToolMedium.png", "Icons.WebBrowserToolLarge.png")]
	[ClickHandler("apply", "Apply")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]

	[ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
	public class WebBrowserTool : Tool<ClearCanvas.Desktop.IDesktopToolContext>
	{
		private bool _enabled;
		private event EventHandler _enabledChanged;

		/// <summary>
		/// Default constructor.  A no-args constructor is required by the
		/// framework.  Do not remove.
		/// </summary>
		public WebBrowserTool()
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
		/// Called by the framework when the user clicks the "apply" menu item or toolbar button.
		/// </summary>
		public void Apply()
		{
			// TODO
			// Add code here to implement the functionality of the tool
			// If this tool is associated with a workspace, you can access the workspace
			// using the Workspace property

			WebBrowserComponent component = new WebBrowserComponent();

			ApplicationComponent.LaunchAsWorkspace(
				this.Context.DesktopWindow,
				component,
				SR.WorkspaceName,
				null);
		}
	}
}
