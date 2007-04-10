using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.AuthenticationAdmin
{
    [DataContract]
    public class UpdateAuthorityGroupRequest : DataContractBase
    {
        public UpdateAuthorityGroupRequest(EntityRef authorityGroupRef, AuthorityGroupDetail authorityGroupDetail)
        {
            AuthorityGroupRef = authorityGroupRef;
            AuthorityGroupDetail = authorityGroupDetail;
        }

        [DataMember]
        public EntityRef AuthorityGroupRef;

        [DataMember]
        public AuthorityGroupDetail AuthorityGroupDetail;
    }
}
