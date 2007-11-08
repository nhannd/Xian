using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ProtocollingWorkflow
{
    [DataContract]
    public class GetProtocolOperationEnablementResponse : DataContractBase
    {
        public GetProtocolOperationEnablementResponse()
        {
            AcceptEnabled = false;
            SuspendEnabled = false;
            RejectEnabled = false;
            SaveEnabled = false;
        }

        [DataMember]
        public bool AcceptEnabled;

        [DataMember]
        public bool SuspendEnabled;

        [DataMember]
        public bool RejectEnabled;

        [DataMember]
        public bool SaveEnabled;
    }
}