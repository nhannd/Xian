using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class GetLinkableInterpretationsResponse : DataContractBase
    {
        public GetLinkableInterpretationsResponse(List<ReportingWorklistItem> interpretationItems)
        {
            this.IntepretationItems = interpretationItems;
        }

        [DataMember]
        public List<ReportingWorklistItem> IntepretationItems;
    }
}
