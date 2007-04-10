using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.AuthenticationAdmin
{
    [DataContract]
    public class LoadUserForEditRequest : DataContractBase
    {
        public LoadUserForEditRequest(EntityRef userRef)
        {
            UserRef = userRef;
        }

        [DataMember]
        public EntityRef UserRef;
    }
}
