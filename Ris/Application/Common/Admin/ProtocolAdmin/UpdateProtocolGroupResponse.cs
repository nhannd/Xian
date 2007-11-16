using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.ProtocolAdmin
{
    [DataContract]
    public class UpdateProtocolGroupResponse : DataContractBase
    {
        public UpdateProtocolGroupResponse(ProtocolGroupSummary summary)
        {
            Summary = summary;
        }

        [DataMember]
        public ProtocolGroupSummary Summary;
    }
}