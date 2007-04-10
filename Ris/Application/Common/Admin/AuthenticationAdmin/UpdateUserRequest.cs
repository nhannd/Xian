using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.AuthenticationAdmin
{
    [DataContract]
    public class UpdateUserRequest : DataContractBase
    {
        public UpdateUserRequest(EntityRef userRef, UserDetail userDetail)
        {
            UserRef = userRef;
            UserDetail = userDetail;
        }

        [DataMember]
        public EntityRef UserRef;

        [DataMember]
        public UserDetail UserDetail;
    }
}
