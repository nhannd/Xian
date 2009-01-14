using System;
using System.Collections.Generic;
using System.Text;
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
