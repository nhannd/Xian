using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ProtocollingWorkflow
{
    [DataContract]
    public class StartOrderProtocolResponse : DataContractBase
    {
        public StartOrderProtocolResponse(bool protocolClaimed)
        {
            ProtocolClaimed = protocolClaimed;
        }

        [DataMember]
        public bool ProtocolClaimed;
    }
}