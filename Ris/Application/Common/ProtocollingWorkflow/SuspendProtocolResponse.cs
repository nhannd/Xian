using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ProtocollingWorkflow
{
    [DataContract]
    public class SuspendProtocolResponse : DataContractBase
    {
        public SuspendProtocolResponse(ProtocolDetail protocolDetail)
        {
            ProtocolDetail = protocolDetail;
        }

        [DataMember]
        public ProtocolDetail ProtocolDetail;
    }
}