using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines the interface that an Application Component sees to it's host.  All methods on this interface
    /// are intended to be called only by the hosted application component.
    /// </summary>
    public interface IApplicationComponentHost
    {
        /// <summary>
        /// Instructs the host to terminate, if for instance, the user has pressed an OK or Cancel button.
        /// The host will subsequently call <see cref="IApplicationComponent.Stop"/>.  Not all hosts
        /// support this method.
        /// </summary>
        void Exit();

        /// <summary>
        /// Asks the host to display a message box to the user.  It is preferable for application
        /// components to use this method, rather than accessing the lower-level platform methods.
        /// </summary>
        /// <param name="message">The message to display</param>
        /// <param name="buttons">The buttons to display</param>
        /// <returns>A result indicating which button the user pressed</returns>
        DialogBoxAction ShowMessageBox(string message, MessageBoxActions buttons);

        /// <summary>
        /// Asks the host to set the title bar text for this component.  Not all hosts support this
        /// method.
        /// </summary>
        /// <param name="title"></param>
        void SetTitle(string title);

        /// <summary>
        /// Provides component with access to the relevant <see cref="CommandHistory"/>.  Not all hosts
        /// support this property.
        /// </summary>
        CommandHistory CommandHistory { get; }

        /// <summary>
        /// Provides component with access to the desktop window in which it is running.
        /// </summary>
        IDesktopWindow DesktopWindow { get; }
    }
}
