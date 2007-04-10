using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.AuthenticationAdmin
{
    [DataContract]
    public class LoadAuthorityGroupForEditResponse : DataContractBase
    {
        public LoadAuthorityGroupForEditResponse(EntityRef authorityGroupRef, AuthorityGroupDetail authorityGroupDetail)
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
