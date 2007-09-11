using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin
{
    [DataContract]
    public class ListRequestedProcedureTypeGroupsForWorklistCategoryResponse : DataContractBase
    {
        public ListRequestedProcedureTypeGroupsForWorklistCategoryResponse()
        {
            RequestedProcedureTypeGroups = new List<RequestedProcedureTypeGroupSummary>();
        }

        [DataMember]
        public List<RequestedProcedureTypeGroupSummary> RequestedProcedureTypeGroups;
    }
}