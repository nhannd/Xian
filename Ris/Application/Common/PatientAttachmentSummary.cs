using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class PatientAttachmentSummary : DataContractBase
    {
        public PatientAttachmentSummary(EnumValueInfo category, MimeDocumentSummary document)
        {
            this.Category = category;
            this.Document = document;
        }

        [DataMember]
        public EnumValueInfo Category;

        [DataMember]
        public MimeDocumentSummary Document;
    }
}
