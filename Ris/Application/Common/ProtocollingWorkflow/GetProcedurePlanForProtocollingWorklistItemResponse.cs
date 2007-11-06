using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ProtocollingWorkflow
{
    [DataContract]
    public class GetProcedurePlanForProtocollingWorklistItemResponse : DataContractBase
    {
        public GetProcedurePlanForProtocollingWorklistItemResponse(ProcedurePlanSummary procedurePlanSummary)
        {
            ProcedurePlanSummary = procedurePlanSummary;
        }

        [DataMember]
        public ProcedurePlanSummary ProcedurePlanSummary;
    }
}