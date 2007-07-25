using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin
{
    [DataContract]
    public class UpdateWorklistRequest : DataContractBase
    {
        public UpdateWorklistRequest(EntityRef entityRef, WorklistAdminDetail detail)
        {
            EntityRef = entityRef;
            Detail = detail;
        }

        [DataMember]
        public EntityRef EntityRef;

        [DataMember]
        public WorklistAdminDetail Detail;
    }
}