#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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
        IDesktopWindowView CreateDesktopWindowView(DesktopWindow window);

        /// <summary>
        /// Displays a message box.
        /// </summary>
        /// <param name="message">The message to display in the mesage box.</param>
        /// <param name="buttons">The buttons to display in the message box.</param>
        /// <returns>The result of the user dismissing the message box.</returns>
        DialogBoxAction ShowMessageBox(string message, MessageBoxActions buttons);
    }
}
