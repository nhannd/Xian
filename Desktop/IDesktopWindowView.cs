using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines the interface to a view for a <see cref="DesktopWindow"/> object.
    /// </summary>
    public interface IDesktopWindowView : IDesktopObjectView
    {
        /// <summary>
        /// Creates a view for the specified workspace.
        /// </summary>
        /// <param name="workspace"></param>
        /// <returns></returns>
        IWorkspaceView CreateWorkspaceView(Workspace workspace);

        /// <summary>
        /// Creates a view for the specified shelf.
        /// </summary>
        /// <param name="shelf"></param>
        /// <returns></returns>
        IShelfView CreateShelfView(Shelf shelf);

        /// <summary>
        /// Creates a view for the specified dialog box.
        /// </summary>
        /// <param name="dialog"></param>
        /// <returns></returns>
        IDialogBoxView CreateDialogBoxView(DialogBox dialog);

        /// <summary>
        /// Sets the current menu model.
        /// </summary>
        /// <param name="model"></param>
        void SetMenuModel(ActionModelNode model);

        /// <summary>
        /// Sets the current toolbar model.
        /// </summary>
        /// <param name="model"></param>
        void SetToolbarModel(ActionModelNode model);

        /// <summary>
        /// Shows a message box in front of this desktop window.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        DialogBoxAction ShowMessageBox(string message, string title, MessageBoxActions buttons);
    }
}
