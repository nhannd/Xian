#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Common.Authentication
{
	[DataContract]
	public class ValidateSessionResponse : DataContractBase
	{
		public ValidateSessionResponse(SessionToken sessionToken, string[] authorityTokens)
		{
			this.SessionToken = sessionToken;
			this.AuthorityTokens = authorityTokens;
		}

		/// <summary>
		/// Updated session token.
		/// </summary>
		[DataMember]
		public SessionToken SessionToken;

		/// <summary>
		/// User authority tokens, if requested.
		/// </summary>
		[DataMember]
		public string[] AuthorityTokens;
	}
}
