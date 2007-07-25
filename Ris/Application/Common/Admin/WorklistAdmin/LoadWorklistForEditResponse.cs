using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin
{
    [DataContract]
    public class LoadWorklistForEditResponse : DataContractBase
    {
        public LoadWorklistForEditResponse(EntityRef entityRef, WorklistAdminDetail detail)
        {
            EntityRef = entityRef;
            this.Detail = detail;
        }

        [DataMember]
        public EntityRef EntityRef;

        [DataMember]
        public WorklistAdminDetail Detail;
    }
}