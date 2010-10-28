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
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Healthcare.Alerts
{

    [SettingsGroupDescription("Configures the behaviour of Healthcare alerts.")]
    [SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
    internal sealed partial class AlertsSettings
    {
        /// <summary>
        /// Public constructor.
        /// </summary>
        /// <remarks>
        /// Server-side settings classes should be instantiated via constructor rather
        /// than using the <see cref="AlertsSettings.Default"/> property to avoid creating a static instance.
        /// </remarks>
        public AlertsSettings()
        {
        }
    }
}
