using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin
{
    [DataContract]
    public class AddWorklistResponse : DataContractBase
    {
        public AddWorklistResponse(WorklistAdminSummary worklistAdminSummary)
        {
            WorklistAdminSummary = worklistAdminSummary;
        }

        [DataMember]
        public WorklistAdminSummary WorklistAdminSummary;
    }
}