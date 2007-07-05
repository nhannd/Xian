using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.RequestedProcedureTypeGroupAdmin
{
    [DataContract]
    public class AddRequestedProcedureTypeGroupResponse : DataContractBase
    {
        public AddRequestedProcedureTypeGroupResponse(RequestedProcedureTypeGroupSummary addedRequestedProcedureTypeGroupSummary)
        {
            AddedRequestedProcedureTypeGroupSummary = addedRequestedProcedureTypeGroupSummary;
        }

        [DataMember]
        public RequestedProcedureTypeGroupSummary AddedRequestedProcedureTypeGroupSummary;
    }
}