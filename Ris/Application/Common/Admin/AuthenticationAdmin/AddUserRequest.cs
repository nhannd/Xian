using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.AuthenticationAdmin
{
    [DataContract]
    public class AddUserRequest : DataContractBase
    {
        public AddUserRequest(UserDetail userDetail)
        {
            UserDetail = userDetail;
        }

        [DataMember]
        public UserDetail UserDetail;
    }
}
