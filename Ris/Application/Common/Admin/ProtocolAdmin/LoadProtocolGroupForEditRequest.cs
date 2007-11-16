using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.ProtocolAdmin
{
    [DataContract]
    public class LoadProtocolGroupForEditRequest : DataContractBase
    {
        [DataMember]
        public EntityRef ProtocolGroupRef;
    }
}