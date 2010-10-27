#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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

		[CommandLineParameter("sname", "Specifies the system admin user display name. Default is 'sysadmin'.")]
		public string SysAdminDisplayName
		{
			get { return _sysAdminDisplayName; }
			set { _sysAdminDisplayName = value; }
		}

		[CommandLineParameter("spwd", "Specifies the system admin user password.  Default is 'clearcanvas'.")]
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
