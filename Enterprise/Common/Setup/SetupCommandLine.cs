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

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Common.Setup
{
	class SetupCommandLine : CommandLine
	{
        private string _userName;
        private string _password;
		private string _sysAdminGroup;
		private bool _importDefaultAuthorityGroups = true;

		/// <summary>
		/// Specifies whether to create default authority groups.
		/// </summary>
		[CommandLineParameter("groups", "g", "Specifies whether to import the default authority groups. This option is enabled by default.")]
		public bool ImportDefaultAuthorityGroups
		{
			get { return _importDefaultAuthorityGroups; }
			set { _importDefaultAuthorityGroups = value; }
		}

		/// <summary>
		/// Specifies user name to connect to enterprise server.
		/// </summary>
		[CommandLineParameter("suid", "Specifies user name to connect to enterprise server.", Required = true)]
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

		/// <summary>
		/// Specifies password to connect to enterprise server.
		/// </summary>
		[CommandLineParameter("spwd", "Specifies password to connect to enterprise server.", Required = true)]
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

		/// <summary>
		/// Name of the sys-admin group. Imported tokens will be automatically added to this group.
		/// </summary>
		[CommandLineParameter("sgroup", "Specifies the name of the system admin authority group, so that imported tokens can be added to it.")]
		public string SysAdminGroup
		{
			get { return _sysAdminGroup; }
			set { _sysAdminGroup = value; }
		}
	}
}
