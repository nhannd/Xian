using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin.AuthenticationAdmin;

namespace ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin
{
    [DataContract]
    public class WorklistAdminDetail : DataContractBase
    {
        public WorklistAdminDetail()
        {
            Users = new List<UserSummary>();
            RequestedProcedureTypeGroups = new List<RequestedProcedureTypeGroupSummary>();
        }


        public WorklistAdminDetail(EntityRef entityRef, string name, string description, string worklistType)
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

        [DataMember]
        public List<UserSummary> Users;
    }
}