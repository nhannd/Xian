using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Application.Common.Admin.AuthenticationAdmin
{
    [DataContract]
    public class UserDetail : DataContractBase
    {
        public UserDetail(string userId, PersonNameDetail userName, List<AuthorityGroupSummary> authorityGroups)
        {
            UserId = userId;
            UserName = userName;
            AuthorityGroups = authorityGroups;
        }

        [DataMember]
        public string UserId;

        [DataMember]
        public PersonNameDetail UserName;

        [DataMember]
        public List<AuthorityGroupSummary> AuthorityGroups;
    }
}
