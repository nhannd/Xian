using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.AuthenticationAdmin
{
    [DataContract]
    public class UserSummary : DataContractBase
    {
        public UserSummary(EntityRef entityRef, string userId)
        {
            EntityRef = entityRef;
            UserId = userId;
        }

        public UserSummary()
        {
        }
        
        [DataMember]
        public EntityRef EntityRef;

        [DataMember]
        public string UserId;
    }
}
