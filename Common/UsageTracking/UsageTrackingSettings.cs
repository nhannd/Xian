#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Configuration;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Common.UsageTracking
{
    [SettingsGroupDescription("Settings that determine if application usage tracking information is sent to a ClearCanvas server.")]
    [SettingsProvider(typeof(ApplicationCriticalSettingsProvider))]
    [SharedSettingsMigrationDisabled]
    internal sealed partial class UsageTrackingSettings
    {
        private UsageTrackingSettings()
        {
        }
    }
}
