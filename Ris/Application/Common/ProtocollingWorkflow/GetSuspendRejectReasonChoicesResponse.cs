using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ProtocollingWorkflow
{
    [DataContract]
    public class GetSuspendRejectReasonChoicesResponse : DataContractBase
    {
        public GetSuspendRejectReasonChoicesResponse(List<EnumValueInfo> suspendRejectReasonChoices)
        {
            SuspendRejectReasonChoices = suspendRejectReasonChoices;
        }

        [DataMember]
        public List<EnumValueInfo> SuspendRejectReasonChoices;
    }
}