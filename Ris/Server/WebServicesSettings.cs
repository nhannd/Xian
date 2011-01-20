#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Configuration;

namespace ClearCanvas.Ris.Server
{
    /// <summary>
    /// Parameters for the web services exposed by this server. Note that these settings
    /// are stored in the app.config file, not in the enterprise config store.
    /// </summary>
    [SettingsGroupDescription("Parameters for the web services exposed by this server.")]
    internal sealed partial class WebServicesSettings
    {
        
        public WebServicesSettings()
        {
        }
        
    }
}
