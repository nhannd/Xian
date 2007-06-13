using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class GetPriorReportRequest : DataContractBase
    {
        public GetPriorReportRequest(EntityRef stepRef)
        {
            this.ReportingStepRef = stepRef;
        }

        [DataMember]
        public EntityRef ReportingStepRef;
    }
}
