using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ProtocollingWorkflow
{
    [DataContract]
    public class ListProtocolCodesResponse : DataContractBase
    {
        [DataMember]
        public List<ProtocolCodeDetail> Codes;
    }
}