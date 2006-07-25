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
        /// The host will subsequently call <see cref="IApplicationComponent.Stop"/>
        /// </summary>
        void Exit();

        /// <summary>
        /// Asks the host to display a message box to the user.  It is preferable for application
        /// components to use this method, rather than accessing the lower-level platform methods.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        DialogBoxAction ShowMessageBox(string message, MessageBoxActions buttons);
    }
}
