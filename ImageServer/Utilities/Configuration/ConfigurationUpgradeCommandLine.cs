#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

namespace ClearCanvas.ImageServer.Utilities.Configuration
{
	/// <summary>
	/// Command line attributes for <see cref="ConfigurationUpgradeApplication"/>.
	/// </summary>
	public class ConfigurationUpgradeCommandLine : CommandLine
	{
		public ConfigurationUpgradeCommandLine()
		{
			Check = false;	
		}

		/// <summary>
		/// Specifies whether the utility runto import authority tokens.
		/// </summary>
		[CommandLineParameter("check", "c", "Check if the version can be upgraded from.")]
		public bool Check
		{ get; set; }

		/// <summary>
		/// Specifies user name to connect to enterprise server.
		/// </summary>
		[CommandLineParameter("version", "Specifies the current verison of the configuration file.", Required = true)]
		public string Version
		{ get; set; }

		/// <summary>
		/// Specifies the current configuration file.
		/// </summary>
		[CommandLineParameter(0, "Specifies the name of the configuration file to upgrade.")]
		public string ConfigurationFile
		{ get; set; }

		/// <summary>
		/// Specifies the new release's configuration file.
		/// </summary>
		[CommandLineParameter(1, "Specifies the path to the current release configuration file.")]
		public string NewConfigFile
		{ get; set; }

		/// <summary>
		/// Specifies the current configuration file.
		/// </summary>
		[CommandLineParameter(2, "Specifies the path to the source configuration file to upgrade.")]
		public string OldConfigFile
		{ get; set; }

		/// <summary>
		/// Specifies the output configuration file.
		/// </summary>
		[CommandLineParameter(3, "Specifies the path to the destination configuration file.")]
		public string OutputConfigFile
		{ get; set; }
	}
}
