using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class ProtocolGroupSummary : DataContractBase
    {
        public ProtocolGroupSummary(EntityRef entityRef, string name, string description)
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
