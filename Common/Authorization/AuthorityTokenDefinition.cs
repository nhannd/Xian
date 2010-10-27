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

namespace ClearCanvas.Common.Authorization
{
	/// <summary>
	/// Describes an authority token.
	/// </summary>
	public class AuthorityTokenDefinition
	{
		private readonly string _token;
		private readonly string _description;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="token"></param>
		/// <param name="description"></param>
		public AuthorityTokenDefinition(string token, string description)
		{
			_token = token;
			_description = description;
		}

		/// <summary>
		/// Gets the token string.
		/// </summary>
		public string Token
		{
			get { return _token; }
		}

		/// <summary>
		/// Gets the token description.
		/// </summary>
		public string Description
		{
			get { return _description; }
		}
	}
}
