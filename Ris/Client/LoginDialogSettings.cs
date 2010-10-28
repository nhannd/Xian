#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Configuration;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Remembers settings for the login dialog.  These settings are stored on the local machine. Although they 
    /// are "user"-scoped settings, they are not actually associated with a RIS user, rather they are associated
    /// with the Windows login of the local machine.
    /// </summary>
    [SettingsGroupDescriptionAttribute("Stores settings for the login dialog on the local machine.")]
    [SettingsProvider(typeof(LocalFileSettingsProvider))]
    internal sealed partial class LoginDialogSettings
    {
        private LoginDialogSettings()
        {
            ApplicationSettingsRegistry.Instance.RegisterInstance(this);
        }
    }
}
