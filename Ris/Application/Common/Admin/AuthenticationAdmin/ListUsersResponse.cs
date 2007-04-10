using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.AuthenticationAdmin
{
    [DataContract]
    public class ListUsersResponse : DataContractBase
    {
        public ListUsersResponse(List<UserSummary> users)
        {
            Users = users;
        }

        [DataMember]
        public List<UserSummary> Users;
    }
}
