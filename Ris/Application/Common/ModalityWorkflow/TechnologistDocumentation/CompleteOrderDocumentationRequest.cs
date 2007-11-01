using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation
{
    [DataContract]
    public class CompleteOrderDocumentationRequest : DataContractBase
    {
        public CompleteOrderDocumentationRequest(EntityRef orderRef)
        {
            OrderRef = orderRef;
        }

        [DataMember]
        public EntityRef OrderRef;
    }
}