using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class GetWorklistResponse : DataContractBase
    {
        public GetWorklistResponse(List<ReportingWorklistItem> worklistItems)
        {
            this.WorklistItems = worklistItems;
        }

        [DataMember]
        public List<ReportingWorklistItem> WorklistItems;
    }
}
