using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.AuthenticationAdmin
{
    [DataContract]
    public class ListAuthorityGroupsResponse : DataContractBase
    {
        public ListAuthorityGroupsResponse(List<AuthorityGroupSummary> authorityGroups)
        {
            AuthorityGroups = authorityGroups;
        }

        [DataMember]
        public List<AuthorityGroupSummary> AuthorityGroups;
    }
}
