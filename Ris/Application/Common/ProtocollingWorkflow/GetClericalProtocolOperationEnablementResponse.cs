using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ProtocollingWorkflow
{
    [DataContract]
    public class GetClericalProtocolOperationEnablementResponse : DataContractBase
    {
        [DataMember]
        public bool CanResolveByResubmit;

        [DataMember]
        public bool CanResolveByCancel;
    }
}