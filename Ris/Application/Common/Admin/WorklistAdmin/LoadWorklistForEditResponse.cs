using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin
{
    [DataContract]
    public class LoadWorklistForEditResponse : DataContractBase
    {
        public LoadWorklistForEditResponse(EntityRef entityRef, WorklistDetail worklistDetail)
        {
            EntityRef = entityRef;
            WorklistDetail = worklistDetail;
        }

        [DataMember]
        public EntityRef EntityRef;

        [DataMember]
        public WorklistDetail WorklistDetail;
    }
}