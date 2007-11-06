using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Common.MimeDocumentService
{
    [DataContract]
    public class GetAttachDocumentFormDataResponse : DataContractBase
    {
        public GetAttachDocumentFormDataResponse(
            List<EnumValueInfo> patientAttachmentCategoryChoices,
            List<EnumValueInfo> orderAttachmentCategoryChoices)
        {
            this.PatientAttachmentCategoryChoices = patientAttachmentCategoryChoices;
            this.OrderAttachmentCategoryChoices = orderAttachmentCategoryChoices;
        }

        [DataMember]
        public List<EnumValueInfo> PatientAttachmentCategoryChoices;

        [DataMember]
        public List<EnumValueInfo> OrderAttachmentCategoryChoices;
    }
}
