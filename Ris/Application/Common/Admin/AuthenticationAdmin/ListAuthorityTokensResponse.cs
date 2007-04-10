using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Application.Common.Admin.AuthenticationAdmin
{
    [DataContract]
    public class ListAuthorityTokensResponse : DataContractBase
    {
        public ListAuthorityTokensResponse(List<AuthorityTokenSummary> authorityTokens)
        {
            AuthorityTokens = authorityTokens;
        }

        [DataMember]
        public List<AuthorityTokenSummary> AuthorityTokens;
    }
}
