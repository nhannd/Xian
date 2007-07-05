using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.RequestedProcedureTypeGroupAdmin
{
    [DataContract]
    public class LoadRequestedProcedureTypeGroupForEditRequest : DataContractBase
    {
        public LoadRequestedProcedureTypeGroupForEditRequest(EntityRef entityRef)
        {
            EntityRef = entityRef;
        }

        [DataMember]
        public EntityRef EntityRef;
    }
}