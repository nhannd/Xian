using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class WorklistSummary : DataContractBase
    {
        public WorklistSummary(EntityRef entityRef, string displayName, string description, string type)
        {
            EntityRef = entityRef;
            DisplayName = displayName;
            Description = description;
            Type = type;
        }

        public WorklistSummary(EntityRef entityRef, string displayName, string description)
            : this(entityRef, displayName, description, "")
        {
        }

        [DataMember]
        public EntityRef EntityRef;

        [DataMember]
        public string DisplayName;

        [DataMember]
        public string Description;

        [DataMember]
        public string Type;
    }
}
