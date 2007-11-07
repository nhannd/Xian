using System;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class MimeDocumentSummary : DataContractBase
    {
        [DataMember]
        public EntityRef DocumentRef;

        [DataMember]
        public DateTime? CreationTime;

        [DataMember]
        public string MimeType;

        [DataMember]
        public string FileExtension;

        [DataMember]
        public EntityRef BinaryDataRef;

        public MimeDocumentSummary As<T>()
            where T : MimeDocumentSummary
        {
            MimeDocumentSummary doc = new MimeDocumentSummary();

            doc.DocumentRef = this.DocumentRef;
            doc.CreationTime = this.CreationTime;
            doc.MimeType = this.MimeType;
            doc.FileExtension = this.FileExtension;
            doc.BinaryDataRef = this.BinaryDataRef;

            return doc;
        }
    }
}
