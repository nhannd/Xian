using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation
{
    [DataContract]
    public class CompleteOrderDocumentationResponse : DataContractBase
    {
        [DataMember]
        public ProcedurePlanSummary ProcedurePlanSummary;
    }
}