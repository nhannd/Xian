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
        [DataMember]
        public int ReportPartIndex;

        [DataMember]
        public ReportSummary Report;
    }
}
