using System;

using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines the interface to a desktop window, as seen by the <see cref="Application"/>
    /// </summary>
    public interface IDesktopWindow : IDisposable
    {
        /// <summary>
        /// Gets the active workspace, or null if there are no workspaces
        /// </summary>
        IWorkspace ActiveWorkspace { get; }

        /// <summary>
        /// Gets the current menu model
        /// </summary>
        ActionModelNode MenuModel { get; }

        /// <summary>
        /// Gets the current toolbar model
        /// </summary>
        ActionModelNode ToolbarModel { get; }

        /// <summary>
        /// Gets the associated shelf manager
        /// </summary>
        ShelfManager ShelfManager { get; }

        /// <summary>
        /// Gets the associated workspace manager
        /// </summary>
        WorkspaceManager WorkspaceManager { get; }

        /// <summary>
        /// Asks the desktop window if it is in a closable state.  The desktop window may take any actions
        /// in this method that are necessary to decide whether or not it can be closed, including prompting
        /// the user to save data, etc.
        /// </summary>
        /// <returns></returns>
        bool CanClose();

        /// <summary>
        /// Shows a message box in this desktop window
        /// </summary>
        /// <param name="message"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        DialogBoxAction ShowMessageBox(string message, MessageBoxActions buttons);

    }
}
