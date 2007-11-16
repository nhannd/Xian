using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.ProtocolAdmin
{
    [DataContract]
    public class AddProtocolGroupResponse : DataContractBase
    {
        public AddProtocolGroupResponse(ProtocolGroupSummary summary)
        {
            Summary = summary;
        }

        [DataMember]
        public ProtocolGroupSummary Summary;
    }
}