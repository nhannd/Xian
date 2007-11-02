using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ProtocollingWorkflow
{
    [DataContract]
    public class ApproveResidentProtocolResponse : DataContractBase
    {
        public ApproveResidentProtocolResponse(ProtocolDetail protocolDetail)
        {
            ProtocolDetail = protocolDetail;
        }

        [DataMember]
        public ProtocolDetail ProtocolDetail;
    }
}