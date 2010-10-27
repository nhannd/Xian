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
    /// Defines the interface to a view for a <see cref="DialogBox"/> object.
    /// </summary>
    public interface IDialogBoxView : IDesktopObjectView
    {
        /// <summary>
        /// Displays the dialog and blocks until the dialog is closed by the user.
        /// </summary>
        /// <returns>A result representing the action taken by the user.</returns>
        DialogBoxAction RunModal();
    }
}
