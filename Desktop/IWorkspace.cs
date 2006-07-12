using System;

using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines the public interface for a workspace.  Implementations should preferably extend the
    /// abstract class <see cref="Workspace"/> rather than implement this interface directly, as
    /// if provides some default functionality.
    /// </summary>
    public interface IWorkspace
    {
        /// <summary>
        /// Indicates that this workspace was closed.  Implementations must fire this
        /// event when a call to <see cref="IWorkspace.Close()"/> succeeds.
        /// </summary>
        event EventHandler WorkspaceClosed;

        /// <summary>
        /// Indicates that the <see cref="IWorkspace.IsActivated"/> property has changed.
        /// </summary>
        event EventHandler<ActivationChangedEventArgs> ActivationChanged;

        /// <summary>
        /// Indicates that the <see cref="IWorkspace.Title"/> property has changed.
        /// </summary>
        event EventHandler TitleChanged;

        /// <summary>
        /// Returns the command history for this workspace
        /// </summary>
        CommandHistory CommandHistory { get; }

        /// <summary>
        /// Returns true if this workspace is currently the active workspace.  Only one workspace
        /// can be active at a given time.
        /// </summary>
        bool IsActivated { get; set; }

        /// <summary>
        /// Returns the title that should be displayed for the workspace in the user-interface
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Returns the public tool-set for this workspace.  The actions corresponding to this
        /// toolset will be integrated into the main menu and toolbars.
        /// </summary>
        IToolSet ToolSet { get; }

        /// <summary>
        /// Returns the view for this workspace
        /// </summary>
        IWorkspaceView View { get; }

        /// <summary>
        /// Asks the workspace to close.  If this method succeeds, it should fire the
        /// <see cref="IWorkspace.WorkspaceClosed"/> event in order to notify the <see cref="WorkspaceManager"/>
        /// that the workspace should be removed.
        /// </summary>
        void Close();
    }
}
