using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin
{
    [DataContract]
    public class ListRequestedProcedureTypeGroupsForWorklistCategoryRequest : DataContractBase
    {
        public ListRequestedProcedureTypeGroupsForWorklistCategoryRequest(string worklistType)
        {
            WorklistType = worklistType;
        }

        [DataMember]
        public string WorklistType;
    }
}