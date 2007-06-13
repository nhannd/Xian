using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class ReportSummary : DataContractBase
    {
        [DataMember]
        public EntityRef ReportRef;

        [DataMember]
        public string ReportContent;

        [DataMember]
        public string DiagnosticServiceName;

        [DataMember]
        public string RequestedProcedureName;


    }
}
