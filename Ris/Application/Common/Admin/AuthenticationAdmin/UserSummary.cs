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


        protected bool Equals(UserSummary userSummary)
        {
            if (userSummary == null) return false;
            return Equals(UserId, userSummary.UserId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as UserSummary);
        }

        public override int GetHashCode()
        {
            return UserId.GetHashCode();
        }
    }
}
