using System.Runtime.Serialization;
using System.Collections.Generic;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.MimeDocumentService
{
    [DataContract]
    public class GetAttachmentsForOrderResponse : DataContractBase
    {
        public GetAttachmentsForOrderResponse(List<OrderAttachmentSummary> attachments)
        {
            this.Attachments = attachments;
        }

        [DataMember]
        public List<OrderAttachmentSummary> Attachments;
    }
}
