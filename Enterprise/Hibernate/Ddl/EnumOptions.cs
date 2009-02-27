using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
	/// <summary>
	/// Defines the set of options for which enumerations to process.
	/// </summary>
	public enum EnumOptions
	{
		/// <summary>
		/// Both hard and soft enumerations should be generated/processed.
		/// </summary>
		all,

		/// <summary>
		/// Only hard enumerations should be generated/processed.
		/// </summary>
		hard,

		/// <summary>
		/// No enumerations should be generated/processed.
		/// </summary>
		none
	}
}
