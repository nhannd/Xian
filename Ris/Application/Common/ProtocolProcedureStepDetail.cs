using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class ProtocolProcedureStepDetail : DataContractBase
    {
        [DataMember]
        public EntityRef ProtocolProcedureStepRef;

        [DataMember]
        public EntityRef ProtocolRef;

        [DataMember]
        public EnumValueInfo Status;
    }
}