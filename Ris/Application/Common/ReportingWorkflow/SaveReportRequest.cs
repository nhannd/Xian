using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class SaveReportRequest : DataContractBase
    {
        public SaveReportRequest(EntityRef stepRef, string reportContent)
        {
            this.ReportingStepRef = stepRef;
            this.ReportContent = reportContent;
        }

        [DataMember]
        public EntityRef ReportingStepRef;

        [DataMember]
        public string ReportContent;
    }
}
