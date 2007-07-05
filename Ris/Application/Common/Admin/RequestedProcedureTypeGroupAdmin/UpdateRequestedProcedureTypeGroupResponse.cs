using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.RequestedProcedureTypeGroupAdmin
{
    [DataContract]
    public class UpdateRequestedProcedureTypeGroupResponse : DataContractBase
    {
        public UpdateRequestedProcedureTypeGroupResponse(RequestedProcedureTypeGroupSummary updatedRequestedProcedureTypeGroupSummary)
        {
            UpdatedRequestedProcedureTypeGroupSummary = updatedRequestedProcedureTypeGroupSummary;
        }

        [DataMember]
        public RequestedProcedureTypeGroupSummary UpdatedRequestedProcedureTypeGroupSummary;
    }
}