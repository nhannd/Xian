using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin
{
    [DataContract]
    public class WorklistAdminSummary : DataContractBase
    {
        public WorklistAdminSummary(EntityRef entityRef, string name, string description, string worklistType)
        {
            EntityRef = entityRef;
            Name = name;
            Description = description;
            WorklistType = worklistType;
        }

        [DataMember]
        public EntityRef EntityRef;

        [DataMember]
        public string Name;

        [DataMember]
        public string Description;

        [DataMember]
        public string WorklistType;
    }
}