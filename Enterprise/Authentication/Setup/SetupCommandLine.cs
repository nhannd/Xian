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

namespace ClearCanvas.Enterprise.Authentication.Setup
{
	class SetupCommandLine : CommandLine
	{
		/// <summary>
		/// Import default groups by default.
		/// </summary>
		private bool _importDefaultAuthorityGroups = true;
		private string _sysAdminUserName = "sa";
		private string _sysAdminDisplayName = "sysadmin";
		private string _sysAdminInitialPassword = "clearcanvas";
		private string _sysAdminGroup = "Administrators";

		/// <summary>
		/// Specifies whether to create default authority groups.
		/// </summary>
		[CommandLineParameter("groups", "g", "Specifies whether to import the default authority groups.")]
		public bool ImportDefaultAuthorityGroups
		{
			get { return _importDefaultAuthorityGroups; }
			set { _importDefaultAuthorityGroups = value; }
		}

		[CommandLineParameter("suid", "Specifies the system admin user-name.  Default is 'sa'.")]
		public string SysAdminUserName
		{
			get { return _sysAdminUserName; }
			set { _sysAdminUserName = value; }
		}

		[CommandLineParameter("sname", "Specifies the initial system admin display name. Default is 'sysadmin'.")]
		public string SysAdminDisplayName
		{
			get { return _sysAdminDisplayName; }
			set { _sysAdminDisplayName = value; }
		}

		[CommandLineParameter("spwd", "Specifies the initial system admin password.  Default is 'clearcanvas'.")]
		public string SysAdminInitialPassword
		{
			get { return _sysAdminInitialPassword; }
			set { _sysAdminInitialPassword = value; }
		}

		[CommandLineParameter("sgroup", "Specifies the name of the system admin authority group.  Default is 'Administrators'.")]
		public string SysAdminGroup
		{
			get { return _sysAdminGroup; }
			set { _sysAdminGroup = value; }
		}
	}
}
