#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Configuration;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Provides application settings for banner.
    /// </summary>
    /// <remarks>
    /// This code is adapted from the Visual Studio generated template code;  the generated code has been removed from the project.  Additional 
    /// settings need to be manually added to this class.
    /// </remarks>
    [SettingsGroupDescription("Settings that configure the display of the patient banner.")]
    [SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
    public sealed class BannerSettings : global::System.Configuration.ApplicationSettingsBase
    {
        private static BannerSettings defaultInstance = ((BannerSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new BannerSettings())));

        private BannerSettings()
        {
            ApplicationSettingsRegistry.Instance.RegisterInstance(this);
        }

        public static BannerSettings Default {
            get {
                return defaultInstance;
            }
        }

        /// <summary>
        /// Defined the height of banner in pixels.
        /// </summary>
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SettingsDescriptionAttribute("Defined the height of banner in pixels.")]
        [global::System.Configuration.DefaultSettingValueAttribute("95")]
        public int BannerHeight {
            get {
                return ((int)(this["BannerHeight"]));
            }
        }
    }
}
