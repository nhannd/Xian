using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin
{
	[DataContract]
	public class ImportAuthorityTokensRequest : DataContractBase
	{
		public ImportAuthorityTokensRequest(List<AuthorityTokenSummary> tokens)
		{
			Tokens = tokens;
		}

		[DataMember]
		public List<AuthorityTokenSummary> Tokens;
	}
}
