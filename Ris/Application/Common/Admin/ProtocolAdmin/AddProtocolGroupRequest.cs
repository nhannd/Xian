using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.ProtocolAdmin
{
    [DataContract]
    public class AddProtocolGroupRequest : DataContractBase
    {
        public AddProtocolGroupRequest(ProtocolGroupDetail detail)
        {
            Detail = detail;
        }

        [DataMember]
        public ProtocolGroupDetail Detail;
    }
}