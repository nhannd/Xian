using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation
{
    [DataContract]
    public class GetProcedureStepsForWorklistItemRequest : DataContractBase
    {
        public GetProcedureStepsForWorklistItemRequest(ModalityWorklistItem worklistItem)
        {
            WorklistItem = worklistItem;
        }

        [DataMember]
        public ModalityWorklistItem WorklistItem;
    }
}