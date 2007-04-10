using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.AuthenticationAdmin
{
    [DataContract]
    public class AddUserResponse : DataContractBase
    {
        public AddUserResponse(EntityRef userRef, UserSummary userSummary)
        {
            UserRef = userRef;
            UserSummary = userSummary;
        }

        [DataMember]
        public EntityRef UserRef;

        [DataMember]
        public UserSummary UserSummary;
    }
}
