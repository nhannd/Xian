using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.ProtocolAdmin
{
    [DataContract]
    public class LoadProtocolGroupForEditRequest : DataContractBase
    {
        public LoadProtocolGroupForEditRequest(EntityRef protocolGroupRef)
        {
            ProtocolGroupRef = protocolGroupRef;
        }

        [DataMember]
        public EntityRef ProtocolGroupRef;
    }
}