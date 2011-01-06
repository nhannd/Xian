#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Configuration;

namespace ClearCanvas.Ris.Client
{

    /// <summary>
    /// Provides URL for RIS application web services.  These settings are stored locally in the app.config file.
    /// </summary>
    [SettingsGroupDescriptionAttribute("Provides location of the RIS application web services.")]
    [SettingsProvider(typeof(LocalFileSettingsProvider))]
    internal sealed partial class WebServicesSettings
    {
        private WebServicesSettings()
        {
        }
    }
}
