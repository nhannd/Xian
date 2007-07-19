using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines the interface to a view for the <see cref="Application"/> object.
    /// </summary>
    public interface IApplicationView : IView
    {
        /// <summary>
        /// Creates a view for the specified desktop window.
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        IDesktopWindowView CreateDesktopWindowView(DesktopWindow window);

        /// <summary>
        /// Displays a message box.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        DialogBoxAction ShowMessageBox(string message, MessageBoxActions buttons);
    }
}
