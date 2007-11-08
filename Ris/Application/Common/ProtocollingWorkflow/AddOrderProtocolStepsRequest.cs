using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ProtocollingWorkflow
{
    [DataContract]
    public class AddOrderProtocolStepsRequest : DataContractBase
    {
        public AddOrderProtocolStepsRequest(EntityRef requestedProcedureRef)
        {
            RequestedProcedureRef = requestedProcedureRef;
        }

        [DataMember]
        public EntityRef RequestedProcedureRef;
    }
}