using System;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class MimeDocumentSummary
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
    }
}
