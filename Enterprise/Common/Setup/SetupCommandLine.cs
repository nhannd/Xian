#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Common.Setup
{
	class SetupCommandLine : CommandLine
	{
		public SetupCommandLine()
		{
			SysAdminGroup = "Administrators";
			Password = "clearcanvas";
			UserName = "sa";
			ImportSettingsGroups = true;
			ImportDefaultAuthorityGroups = true;
			ImportAuthorityTokens = true;
			MigrateSharedSettings = true;
		}

        /// <summary>
        /// Specifies whether to import authority tokens.
        /// </summary>
        [CommandLineParameter("tokens", "t", "Specifies whether to import authority tokens. This option is enabled by default.")]
		public bool ImportAuthorityTokens { get; set; }

		/// <summary>
		/// Specifies whether to create default authority groups.
		/// </summary>
		[CommandLineParameter("groups", "g", "Specifies whether to import the default authority groups. This option is enabled by default.")]
		public bool ImportDefaultAuthorityGroups { get; set; }

        /// <summary>
        /// Specifies whether to import settings groups.
        /// </summary>
        [CommandLineParameter("settings", "s", "Specifies whether to import settings groups. This option is enabled by default.")]
		public bool ImportSettingsGroups { get; set; }

        /// <summary>
		/// Specifies whether to import settings groups.
		/// </summary>
		[CommandLineParameter("migrate", "m", "Specifies whether to migrate shared settings from a previously installed version.")]
		public bool MigrateSharedSettings { get; set; }

		/// <summary>
		/// Specifies the filename of the previous local configuration file.
		/// </summary>
		[CommandLineParameter("previousConfig", "p", "Specifies the filename of the previous local configuration file.")]
		public string PreviousExeConfigFilename { get; set; }

		/// <summary>
		/// Specifies user name to connect to enterprise server.
		/// </summary>
		[CommandLineParameter("suid", "Specifies user name to connect to enterprise server. Default is 'sa'.")]
		public string UserName { get; set; }

		/// <summary>
		/// Specifies password to connect to enterprise server.
		/// </summary>
		[CommandLineParameter("spwd", "Specifies password to connect to enterprise server. Default is 'clearcanvas'.")]
		public string Password { get; set; }

		/// <summary>
		/// Name of the sys-admin group. Imported tokens will be automatically added to this group.
		/// </summary>
		[CommandLineParameter("sgroup", "Specifies the name of the system admin authority group, so that imported tokens can be added to it.")]
		public string SysAdminGroup { get; set; }
		}
}
