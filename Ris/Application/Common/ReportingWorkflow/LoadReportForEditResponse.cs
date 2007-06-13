using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class LoadReportForEditResponse : DataContractBase
    {
        public LoadReportForEditResponse(string reportContent)
        {
            this.ReportContent = reportContent;
        }

        [DataMember]
        public string ReportContent;
    }
}
