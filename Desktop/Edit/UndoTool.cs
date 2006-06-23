using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Workstation.Model;

namespace ClearCanvas.Workstation.Edit
{
    [MenuAction("activate", "MenuEdit/MenuEditUndo")]
    [ButtonAction("activate", "ToolbarStandard/ToolbarStandardUndo")]
    [ClickHandler("activate", "Activate")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[IconSet("activate", IconScheme.Colour, "", "Icons.UndoMedium.png", "Icons.UndoLarge.png")]
    [Tooltip("activate", "ToolbarStandardUndo")]
    
    /// <summary>
	/// Summary description for Undo.
	/// </summary>
    [ClearCanvas.Common.ExtensionOf(typeof(ClearCanvas.Workstation.Model.ImageWorkspaceToolExtensionPoint))]
	public class UndoTool : Tool
	{
		private bool _enabled = false;
		private event EventHandler _enabledChangedEvent;
		private Workspace _workspace;

		public UndoTool()
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
            _workspace.CommandHistory.Undo();
		}

		private void OnCurrentCommandChanged(object sender, EventArgs e)
		{
			// As long as we're not at the beginning of the history, we can undo
			if (_workspace.CommandHistory.CurrentCommandIndex != -1)
				this.Enabled = true;
			else
				this.Enabled = false;
		}
	}
}
