#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin
{
	[DataContract]
	public class ImportAuthorityTokensRequest : DataContractBase
	{
		public ImportAuthorityTokensRequest(List<AuthorityTokenSummary> tokens)
		{
			Tokens = tokens;
			AddToGroups = new List<string>();
		}


		public ImportAuthorityTokensRequest(List<AuthorityTokenSummary> tokens, List<string> addToGroups)
		{
			Tokens = tokens;
			AddToGroups = addToGroups;
		}

		/// <summary>
		/// Tokens to import.
		/// </summary>
		[DataMember]
		public List<AuthorityTokenSummary> Tokens;

		/// <summary>
		/// Existing authority groups to which the tokens should be added.
		/// </summary>
		[DataMember]
		public List<string> AddToGroups;
	}
}
