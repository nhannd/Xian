using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin
{
    [DataContract]
    public class UpdateWorklistResponse : DataContractBase
    {
        public UpdateWorklistResponse(WorklistSummary worklistSummary)
        {
            WorklistSummary = worklistSummary;
        }

        [DataMember]
        public WorklistSummary WorklistSummary;
    }
}