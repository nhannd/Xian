using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class GetPriorReportResponse : DataContractBase
    {
        public GetPriorReportResponse(List<ReportSummary> reports)
        {
            this.Reports = reports;
        }

        [DataMember]
        public List<ReportSummary> Reports;
    }
}
