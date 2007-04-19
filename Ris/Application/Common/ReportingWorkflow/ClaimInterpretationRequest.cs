using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class ClaimInterpretationRequest : DataContractBase
    {
        public ClaimInterpretationRequest(ReportingWorklistItem item)
        {
            this.WorklistItem = item;
        }

        [DataMember]
        public ReportingWorklistItem WorklistItem;
    }
}