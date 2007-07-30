using System;

using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines the public interface to a <see cref="DesktopWindow"/>.
    /// </summary>
    public interface IDesktopWindow : IDesktopObject
    {
        /// <summary>
        /// Gets the collection of workspaces associated with this window.
        /// </summary>
        WorkspaceCollection Workspaces { get; }

        /// <summary>
        /// Gets the currently active workspace, or null if there are no workspaces.
        /// </summary>
        Workspace ActiveWorkspace { get; }

        /// <summary>
        /// Gets the collection of shelves associated with this window.
        /// </summary>
        ShelfCollection Shelves { get; }

        /// <summary>
        /// Shows a message box in front of this window.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        DialogBoxAction ShowMessageBox(string message, MessageBoxActions buttons);

        /// <summary>
        /// Shows a message box in front of this window.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        DialogBoxAction ShowMessageBox(string message, string title, MessageBoxActions buttons);

        /// <summary>
        /// Shows a dialog box in front of this window.
        /// </summary>
        /// <param name="component"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        DialogBoxAction ShowDialogBox(IApplicationComponent component, string title);

        /// <summary>
        /// Shows a dialog box in front of this window.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        DialogBoxAction ShowDialogBox(DialogBoxCreationArgs args);
    }
}
