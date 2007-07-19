using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines the interface to a view for a <see cref="DialogBox"/> object.
    /// </summary>
    public interface IDialogBoxView : IDesktopObjectView
    {
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
