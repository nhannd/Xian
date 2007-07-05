using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.RequestedProcedureTypeGroupAdmin
{
    [DataContract]
    public class LoadRequestedProcedureTypeGroupForEditResponse : DataContractBase
    {
        public LoadRequestedProcedureTypeGroupForEditResponse(EntityRef entityRef, RequestedProcedureTypeGroupDetail detail)
        {
            EntityRef = entityRef;
            Detail = detail;
        }

        [DataMember]
        public EntityRef EntityRef;

        [DataMember]
        public RequestedProcedureTypeGroupDetail Detail;
    }
}