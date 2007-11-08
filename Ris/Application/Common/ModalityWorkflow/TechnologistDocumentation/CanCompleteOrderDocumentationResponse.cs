using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation
{
    [DataContract]
    public class CanCompleteOrderDocumentationResponse : DataContractBase
    {
        public CanCompleteOrderDocumentationResponse(bool canComplete)
        {
            CanComplete = canComplete;
        }

        [DataMember]
        public bool CanComplete;
    }
}