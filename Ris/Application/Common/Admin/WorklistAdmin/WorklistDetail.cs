using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin
{
    [DataContract]
    public class WorklistDetail : DataContractBase
    {
        public WorklistDetail()
        {
            RequestedProcedureTypeGroups = new List<RequestedProcedureTypeGroupSummary>();
        }


        public WorklistDetail(EntityRef entityRef, string name, string description, string worklistType)
            : this()
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

        [DataMember]
        public List<RequestedProcedureTypeGroupSummary> RequestedProcedureTypeGroups;
    }
}