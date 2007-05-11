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
            Name = userName;
            AuthorityGroups = authorityGroups;
        }

        public UserDetail()
        {
            Name = new PersonNameDetail();
            AuthorityGroups = new List<AuthorityGroupSummary>();
        }

        [DataMember]
        public string UserId;

        [DataMember]
        public PersonNameDetail Name;

        [DataMember]
        public List<AuthorityGroupSummary> AuthorityGroups;

        [DataMember]
        public EntityRef StaffRef;
    }
}
