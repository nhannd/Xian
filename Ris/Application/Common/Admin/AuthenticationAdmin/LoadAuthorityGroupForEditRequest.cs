using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.AuthenticationAdmin
{
    [DataContract]
    public class LoadAuthorityGroupForEditRequest : DataContractBase
    {
        public LoadAuthorityGroupForEditRequest(EntityRef authorityGroupRef)
        {
            AuthorityGroupRef = authorityGroupRef;
        }

        [DataMember]
        public EntityRef AuthorityGroupRef;
    }
}
