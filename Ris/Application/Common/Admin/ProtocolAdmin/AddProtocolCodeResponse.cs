using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.ProtocolAdmin
{
    [DataContract]
    public class AddProtocolCodeResponse : DataContractBase
    {
        public AddProtocolCodeResponse(ProtocolCodeDetail detail)
        {
            Detail = detail;
        }

        [DataMember]
        public ProtocolCodeDetail Detail;
    }
}