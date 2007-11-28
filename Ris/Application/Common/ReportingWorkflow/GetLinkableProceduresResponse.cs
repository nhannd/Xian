using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class GetLinkableProceduresResponse : DataContractBase
    {
        public GetLinkableProceduresResponse(List<RequestedProcedureSummary> procedures)
        {
            this.Procedures = procedures;
        }

        public List<RequestedProcedureSummary> Procedures;
    }
}
