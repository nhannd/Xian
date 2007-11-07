using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.MimeDocumentService
{
    [DataContract]
    public class GetAttachmentsForOrderRequest : DataContractBase
    {
        public GetAttachmentsForOrderRequest(EntityRef orderRef)
        {
            this.OrderRef = orderRef;
        }

        [DataMember]
        public EntityRef OrderRef;
    }
}
