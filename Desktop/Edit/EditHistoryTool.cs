using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Edit
{
    [MenuAction("undo", "global-menus/MenuEdit/MenuEditUndo")]
    [ButtonAction("undo", "global-toolbars/ToolbarStandard/ToolbarStandardUndo")]
    [ClickHandler("undo", "Undo")]
    [EnabledStateObserver("undo", "UndoEnabled", "UndoEnabledChanged")]
    [IconSet("undo", IconScheme.Colour, "", "Icons.UndoMedium.png", "Icons.UndoLarge.png")]
    [Tooltip("undo", "ToolbarStandardUndo")]

    [MenuAction("redo", "global-menus/MenuEdit/MenuEditRedo")]
    [ButtonAction("redo", "global-toolbars/ToolbarStandard/ToolbarStandardRedo")]
    [ClickHandler("redo", "Redo")]
    [EnabledStateObserver("redo", "RedoEnabled", "RedoEnabledChanged")]
    [IconSet("redo", IconScheme.Colour, "", "Icons.RedoMedium.png", "Icons.RedoLarge.png")]
    [Tooltip("redo", "ToolbarStandardRedo")]

    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class EditHistoryTool : Tool<IDesktopToolContext>
    {
        private bool _undoEnabled;
        private event EventHandler _undoEnabledChangedEvent;

        private bool _redoEnabled;
        private event EventHandler _redoEnabledChangedEvent;

        /// <summary>
        /// Constructor
        /// </summary>
        public EditHistoryTool()
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            // need to monitor which workspace is active
            this.Context.DesktopWindow.WorkspaceManager.ActiveWorkspaceChanged += new EventHandler<WorkspaceActivationChangedEventArgs>(WorkspaceActivatedEventHandler);
        }

        public bool UndoEnabled
        {
            get { return _undoEnabled; }
            private set
            {
                if (value != _undoEnabled)
                {
                    _undoEnabled = value;
                    EventsHelper.Fire(_undoEnabledChangedEvent, this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler UndoEnabledChanged
        {
            add { _undoEnabledChangedEvent += value; }
            remove { _undoEnabledChangedEvent -= value; }
        }

        public bool RedoEnabled
        {
            get { return _redoEnabled; }
            private set
            {
                if (value != _redoEnabled)
                {
                    _redoEnabled = value;
                    EventsHelper.Fire(_redoEnabledChangedEvent, this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler RedoEnabledChanged
        {
            add { _redoEnabledChangedEvent += value; }
            remove { _redoEnabledChangedEvent -= value; }
        }


        public void Undo()
        {
            this.ActiveWorkspace.CommandHistory.Undo();
        }

        public void Redo()
        {
            this.ActiveWorkspace.CommandHistory.Redo();
        }

        private IWorkspace ActiveWorkspace
        {
            get { return this.Context.DesktopWindow.ActiveWorkspace; }
        }

        private void WorkspaceActivatedEventHandler(object sender, WorkspaceActivationChangedEventArgs e)
        {
            if (e.DeactivatedWorkspace != null)
            {
                // stop listening to the previously active workspace
                e.DeactivatedWorkspace.CommandHistory.CurrentCommandChanged -= CurrentCommandChangedEventHandler;
            }

            if (e.ActivatedWorkspace != null)
            {
                // listen for changes to the command history of this workspace
                e.ActivatedWorkspace.CommandHistory.CurrentCommandChanged += CurrentCommandChangedEventHandler;
            }

            UpdateEnablement();
        }

        private void CurrentCommandChangedEventHandler(object sender, EventArgs e)
        {
            UpdateEnablement();
        }

        private void UpdateEnablement()
        {
            this.UndoEnabled = (this.ActiveWorkspace != null && this.ActiveWorkspace.CommandHistory.CurrentCommandIndex > -1);
            this.RedoEnabled = (this.ActiveWorkspace != null && this.ActiveWorkspace.CommandHistory.CurrentCommandIndex < this.ActiveWorkspace.CommandHistory.LastCommandIndex);
        }
    }
}
