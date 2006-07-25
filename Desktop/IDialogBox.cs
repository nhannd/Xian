using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines a general interface for a dialog box that is independent of the windowing toolkit that is used
    /// to display it.
    /// </summary>
    public interface IDialogBox
    {
        /// <summary>
        /// Notifies that the dialog is about to close
        /// </summary>
        event EventHandler<ClosingEventArgs> DialogClosing;

        /// <summary>
        /// Initializes the dialog box.
        /// </summary>
        /// <param name="title">The title to be displayed in the title bar</param>
        /// <param name="view">The view that supplies the content for the dialog</param>
        void Initialize(string title, IView view);

        /// <summary>
        /// Displays the dialog and blocks until the dialog is closed by the user.
        /// </summary>
        /// <returns>A result representing the action taken by the user</returns>
        DialogBoxAction RunModal();

        /// <summary>
        /// Terminates the modal loop with the specified action.
        /// </summary>
        /// <param name="action"></param>
        void EndModal(DialogBoxAction action);
    }
}
