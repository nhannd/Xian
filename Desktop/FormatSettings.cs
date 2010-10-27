#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Configuration;

namespace ClearCanvas.Desktop
{
    [SettingsGroupDescription("Configures display format of common items such as dates, times, etc.")]
    [SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
    internal sealed partial class FormatSettings
    {
        private FormatSettings()
        {
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
        }
    }
}
