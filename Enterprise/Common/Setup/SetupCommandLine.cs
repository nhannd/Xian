#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
