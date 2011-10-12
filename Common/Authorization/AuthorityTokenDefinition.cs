#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Common.Authorization
{
	/// <summary>
	/// Describes an authority token.
	/// </summary>
	public class AuthorityTokenDefinition
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="token"></param>
		/// <param name="definingAssembly"></param>
		/// <param name="description"></param>
		/// <param name="formerIdentities"></param>
		public AuthorityTokenDefinition(string token, string definingAssembly, string description, string[] formerIdentities)
		{
			Token = token;
			DefiningAssembly = definingAssembly;
			Description = description;
			FormerIdentities = formerIdentities;
		}

		/// <summary>
		/// Gets the token string.
		/// </summary>
		public string Token { get; private set; }

		/// <summary>
		/// Gets the name of the assembly that defines the token.
		/// </summary>
		public string DefiningAssembly { get; private set; }

		/// <summary>
		/// Gets the token description.
		/// </summary>
		public string Description { get; private set; }

		/// <summary>
		/// Gets the set of former identities for this token, for migration purposes.
		/// </summary>
		public string[] FormerIdentities { get; private set; }
	}
}
