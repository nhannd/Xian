using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ProtocollingWorkflow
{
    [DataContract]
    public class AcceptProtocolResponse : DataContractBase
    {
        public AcceptProtocolResponse(ProtocolDetail protocolDetail)
        {
            ProtocolDetail = protocolDetail;
        }

        [DataMember]
        public ProtocolDetail ProtocolDetail;
    }
}