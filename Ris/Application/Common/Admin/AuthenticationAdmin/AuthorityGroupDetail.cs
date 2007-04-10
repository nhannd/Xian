using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Application.Common.Admin.AuthenticationAdmin
{
    [DataContract]
    public class AuthorityGroupDetail : DataContractBase
    {
        public AuthorityGroupDetail(string name, List<AuthorityTokenSummary> authorityTokens)
        {
            Name = name;
            AuthorityTokens = authorityTokens;
        }

        public AuthorityGroupDetail()
        {
            AuthorityTokens = new List<AuthorityTokenSummary>();
        }

        [DataMember]
        public string Name;

        [DataMember]
        public List<AuthorityTokenSummary> AuthorityTokens;
    }
}
