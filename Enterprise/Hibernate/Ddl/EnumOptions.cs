#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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
		All,

		/// <summary>
		/// Only hard enumerations should be generated/processed.
		/// </summary>
		Hard,

		/// <summary>
		/// No enumerations should be generated/processed.
		/// </summary>
		None
	}
}
