using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.AuthenticationAdmin
{
    [DataContract]
    public class UpdateUserResponse : DataContractBase
    {
        public UpdateUserResponse(UserSummary userSummary)
        {
            UserSummary = userSummary;
        }

        [DataMember]
        public UserSummary UserSummary;
    }
}
