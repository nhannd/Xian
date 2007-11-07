using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.MimeDocumentService
{
    [DataContract]
    public class GetDocumentDataResponse : DataContractBase
    {
        public GetDocumentDataResponse(byte[] binaryData)
        {
            this.BinaryData = binaryData;
        }

        [DataMember]
        public byte[] BinaryData;
    }
}
