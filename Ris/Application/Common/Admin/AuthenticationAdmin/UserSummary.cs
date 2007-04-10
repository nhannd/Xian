using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.AuthenticationAdmin
{
    [DataContract]
    public class UserSummary : DataContractBase
    {
        public UserSummary(EntityRef entityRef, string userId, PersonNameDetail userName)
        {
            EntityRef = entityRef;
            UserId = userId;
            UserName = userName;
        }
        
        [DataMember]
        public EntityRef EntityRef;

        [DataMember]
        public string UserId;

        [DataMember]
        public PersonNameDetail UserName;
    }
}
