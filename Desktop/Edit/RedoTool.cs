using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Workstation.Model;

namespace ClearCanvas.Workstation.Edit
{
    [MenuAction("activate", "MenuEdit/MenuEditRedo")]
    [ButtonAction("activate", "ToolbarStandard/ToolbarStandardRedo")]
    [ClickHandler("activate", "Activate")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[IconSet("activate", IconScheme.Colour, "", "Icons.RedoMedium.png", "Icons.RedoLarge.png")]
    [Tooltip("activate", "ToolbarStandardRedo")]

	/// <summary>
	/// Summary description for Redo.
	/// </summary>
    [ClearCanvas.Common.ExtensionOf(typeof(ClearCanvas.Workstation.Model.ImageWorkspaceToolExtensionPoint))]
	public class RedoTool : Tool
	{
		private bool _enabled = false;
		private event EventHandler _enabledChangedEvent;
		private Workspace _workspace;

		public RedoTool()
		{
		}

		public override void Initialize()
		{
			_workspace = (this.Context as ImageWorkspaceToolContext).Workspace;
			_workspace.CommandHistory.CurrentCommandChanged += new EventHandler(OnCurrentCommandChanged);
		}

		public bool Enabled
		{
			get { return _enabled; }
			private set
			{
				_enabled = value;
				EventsHelper.Fire(_enabledChangedEvent, this, EventArgs.Empty);
			}
		}

		public event EventHandler EnabledChanged
		{
			add { _enabledChangedEvent += value; }
			remove { _enabledChangedEvent -= value; }
		}

		public void Activate()
		{
            _workspace.CommandHistory.Redo();
		}

		private void OnCurrentCommandChanged(object sender, EventArgs e)
		{
			// As long as we're not at the end of the history, we can redo
			if (_workspace.CommandHistory.CurrentCommandIndex !=
				_workspace.CommandHistory.LastCommandIndex)
				this.Enabled = true;
			else
				this.Enabled = false;
		}
	}
}
