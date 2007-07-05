using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.RequestedProcedureTypeGroupAdmin
{
    [DataContract]
    public class ListRequestedProcedureTypeGroupsResponse : DataContractBase
    {
        public ListRequestedProcedureTypeGroupsResponse()
        {
            Items = new List<RequestedProcedureTypeGroupSummary>();
        }

        [DataMember] 
        public List<RequestedProcedureTypeGroupSummary> Items;
    }
}