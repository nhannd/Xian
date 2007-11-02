using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ProtocollingWorkflow
{
    [DataContract]
    public class RejectProtocolResponse : DataContractBase
    {
        public RejectProtocolResponse(ProtocolDetail protocolDetail)
        {
            ProtocolDetail = protocolDetail;
        }

        [DataMember]
        public ProtocolDetail ProtocolDetail;
    }
}