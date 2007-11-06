using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.MimeDocumentService
{
    [DataContract]
    public class GetDocumentDataRequest : DataContractBase
    {
        public GetDocumentDataRequest(EntityRef documentDataRef)
        {
            this.DocumentDataRef = documentDataRef;
        }

        [DataMember]
        public EntityRef DocumentDataRef;
    }
}
