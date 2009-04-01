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
