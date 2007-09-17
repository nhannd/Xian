using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class SearchResponse : DataContractBase
    {
        public SearchResponse(List<ReportingWorklistItem> worklistItems)
        {
            this.WorklistItems = worklistItems;
        }

        [DataMember]
        public List<ReportingWorklistItem> WorklistItems;
    }
}
