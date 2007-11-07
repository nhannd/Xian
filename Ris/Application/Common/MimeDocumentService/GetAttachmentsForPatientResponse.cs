using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.MimeDocumentService
{
    [DataContract]
    public class GetAttachmentsForPatientResponse : DataContractBase
    {
        public GetAttachmentsForPatientResponse(List<PatientAttachmentSummary> attachments)
        {
            this.Attachments = attachments;
        }

        [DataMember]
        public List<PatientAttachmentSummary> Attachments;
    }
}
