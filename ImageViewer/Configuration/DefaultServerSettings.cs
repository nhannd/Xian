#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Configuration;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.ImageViewer.Configuration
{
	[SettingsGroupDescription("Stores a list of default servers for the application.")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class DefaultServerSettings : ApplicationSettingsBase, IMigrateSettings
	{
        private static DefaultServerSettings _default = ((DefaultServerSettings)Synchronized(new DefaultServerSettings()));
        
        private DefaultServerSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}

	    public static DefaultServerSettings Default
	    {
	        get { return _default; }
	    }

	    /// <summary>
        /// Server tree paths to the user&apos;s default servers.
        /// </summary>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Configuration.SettingsDescriptionAttribute("Server tree paths to the user\'s default servers.")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.Collections.Specialized.StringCollection DefaultServerPaths
        {
            get
            {
                return ((global::System.Collections.Specialized.StringCollection)(this["DefaultServerPaths"]));
            }
            set
            {
                this["DefaultServerPaths"] = value;
            }
        }

        #region IMigrateSettings Members

        public void MigrateSettingsProperty(SettingsPropertyMigrationValues migrationValues)
        {
            //TODO (Marmot):
        }

        #endregion
    }
}