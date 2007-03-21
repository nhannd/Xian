using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
    [DataContract]
    public class LoadDiagnosticServiceBreakdownResponse : DataContractBase
    {
        public LoadDiagnosticServiceBreakdownResponse(DiagnosticServiceDetail detail)
        {
            this.DiagnosticServiceDetail = detail;
        }

        [DataMember]
        public DiagnosticServiceDetail DiagnosticServiceDetail;
    }
}
