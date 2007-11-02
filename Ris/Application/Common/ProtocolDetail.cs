using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class ProtocolDetail : DataContractBase
    {
        [DataMember]
        public string Foo;

        [DataMember]
        public List<ProtocolCodeDetail> Codes;
    }
}