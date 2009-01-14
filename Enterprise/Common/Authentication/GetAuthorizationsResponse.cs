using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Common.Authentication
{
	[DataContract]
	public class GetAuthorizationsResponse : DataContractBase
	{
		public GetAuthorizationsResponse(string[] authorityTokens)
		{
			AuthorityTokens = authorityTokens;
		}

		[DataMember]
		public string[] AuthorityTokens;
	}
}
