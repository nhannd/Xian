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

		/// <summary>
		/// Specifies whether to create default authority groups.
		/// </summary>
		[CommandLineParameter("groups", "g", "Specifies whether to import the default authority groups.")]
		public bool ImportDefaultAuthorityGroups
		{
			get { return _importDefaultAuthorityGroups; }
			set { _importDefaultAuthorityGroups = value; }
		}
	}
}
