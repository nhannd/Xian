using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.AuthenticationAdmin
{
    [DataContract]
    public class AuthorityGroupSummary : DataContractBase
    {
        public AuthorityGroupSummary(EntityRef entityRef, string name)
        {
            EntityRef = entityRef;
            Name = name;
        }

        [DataMember]
        public EntityRef EntityRef;

        [DataMember]
        public string Name;
    }
}
