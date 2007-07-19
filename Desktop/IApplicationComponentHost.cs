using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines the interface to an application component host as seen by the hosted application component.
    /// </summary>
    public interface IApplicationComponentHost
    {
        /// <summary>
        /// Instructs the host to terminate, if for instance, the user has pressed an OK or Cancel button.
        /// </summary>
        /// <remarks>
        /// The host will subsequently call <see cref="IApplicationComponent.Stop"/>.  Not all hosts
        /// support this method.
        /// </remarks>
        void Exit();

        /// <summary>
        /// Asks the host to display a message box to the user.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="buttons">The buttons to display.</param>
        /// <returns>A result indicating which button the user pressed.</returns>
        DialogBoxAction ShowMessageBox(string message, MessageBoxActions buttons);

        /// <summary>
        /// Asks the host to set the title for this component in the UI.
        /// </summary>
        /// <remarks>
        /// Not all hosts support this method.
        /// </remarks>
        /// <param name="title"></param>
        void SetTitle(string title);

        /// <summary>
        /// Gets the <see cref="CommandHistory"/> object associated with this host.
        /// </summary>
        /// <remarks>
        /// Not all hosts support this method.
        /// </remarks>
        CommandHistory CommandHistory { get; }

        /// <summary>
        /// Gets the <see cref="DesktopWindow"/> associated with this host.
        /// </summary>
        DesktopWindow DesktopWindow { get; }
    }
}
