using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.AuthenticationAdmin
{
    [DataContract]
    public class AddAuthorityGroupRequest : DataContractBase
    {
        public AddAuthorityGroupRequest(AuthorityGroupDetail authorityGroupDetail)
        {
            AuthorityGroupDetail = authorityGroupDetail;
        }

        [DataMember]
        public AuthorityGroupDetail AuthorityGroupDetail;
    }
}
