using System;

using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines the interface to a workspace as seen by the desktop.
    /// </summary>
    public interface IWorkspace : IDisposable
    {
        /// <summary>
        /// Indicates that the <see cref="IWorkspace.Title"/> property has changed.
        /// </summary>
        event EventHandler TitleChanged;

        /// <summary>
        /// Called by the framework when the workspace is added to a <see cref="WorkspaceManager"/>
        /// </summary>
        void Initialize(IDesktopWindow desktopWindow);

        /// <summary>
        /// Gets the desktop window with which the workspace is associated.
        /// </summary>
        IDesktopWindow DesktopWindow { get; }

        /// <summary>
        /// Returns the command history for this workspace
        /// </summary>
        CommandHistory CommandHistory { get; }

        /// <summary>
        /// Returns true if this workspace is currently the active workspace.  Only one workspace
        /// can be active at a given time.
        /// </summary>
        bool Active { get; }

        /// <summary>
        /// Attempts to make this workspace the active workspace.
        /// </summary>
        void Activate();

        /// <summary>
        /// Returns the title that should be displayed for the workspace in the user-interface
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Returns the current action set for this workspace.  These actions
        /// will be integrated into the main menu and toolbars.
        /// </summary>
        IActionSet Actions { get; }

        /// <summary>
        /// Asks the if it can be closed at this time.  This method should take any necessary action,
        /// such as asking the user if changes should be saved or discarded, in order to answer
        /// the question.
        /// </summary>
        bool CanClose();
    }
}
