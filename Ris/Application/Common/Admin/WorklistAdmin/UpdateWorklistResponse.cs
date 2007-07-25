using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin
{
    [DataContract]
    public class UpdateWorklistResponse : DataContractBase
    {
        public UpdateWorklistResponse(WorklistAdminSummary worklistAdminSummary)
        {
            WorklistAdminSummary = worklistAdminSummary;
        }

        [DataMember]
        public WorklistAdminSummary WorklistAdminSummary;
    }
}