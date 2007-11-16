using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.ProtocolAdmin
{
    [DataContract]
    public class LoadProtocolGroupForEditResponse : DataContractBase
    {
        public LoadProtocolGroupForEditResponse(EntityRef protocolGroupRef, ProtocolGroupDetail detail)
        {
            ProtocolGroupRef = protocolGroupRef;
            Detail = detail;
        }

        [DataMember]
        public EntityRef ProtocolGroupRef;

        [DataMember]
        public ProtocolGroupDetail Detail;
    }
}