using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class LoadReportForEditRequest : DataContractBase
    {
        public LoadReportForEditRequest(EntityRef reportingStepRef)
        {
            this.ReportingStepRef = reportingStepRef;
        }

        [DataMember]
        public EntityRef ReportingStepRef;
    }
}
