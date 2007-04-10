using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.AuthenticationAdmin
{
    [DataContract]
    public class AuthorityTokenSummary : DataContractBase
    {
        public AuthorityTokenSummary(EntityRef entityRef, string name, string description)
        {
            EntityRef = entityRef;
            Name = name;
            Description = description;
        }

        [DataMember]
        public EntityRef EntityRef;

        [DataMember]
        public string Name;

        [DataMember]
        public string Description;
    }
}
