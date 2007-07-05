using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.RequestedProcedureTypeGroupAdmin
{
    [DataContract]
    public class AddRequestedProcedureTypeGroupRequest : DataContractBase
    {
        public AddRequestedProcedureTypeGroupRequest(RequestedProcedureTypeGroupDetail detail)
        {
            Detail = detail;
        }

        [DataMember]
        public RequestedProcedureTypeGroupDetail Detail;
    }
}