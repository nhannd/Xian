#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Common.Authorization
{
	/// <summary>
	/// Attribute used to define authority group tokens on types.
	/// </summary>
	/// <seealso cref="AuthorityGroupDefinition"/>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
	public class AuthorityTokenAttribute : Attribute
	{
		/// <summary>
		/// The token description.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// The former identities of this token, used for purposes of migrating a token.
		/// Separate multiple former identities by a semicolon.
		/// </summary>
		public string Formerly { get; set; }
	}
}
