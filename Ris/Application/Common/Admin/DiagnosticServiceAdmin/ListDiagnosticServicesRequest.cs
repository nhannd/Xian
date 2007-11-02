using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.DiagnosticServiceAdmin
{
    [DataContract]
    public class ListDiagnosticServicesRequest : PagedDataContractBase
    {
        public ListDiagnosticServicesRequest()
        {

        }

        public ListDiagnosticServicesRequest(string diagnosticServiceName, string diagnosticServiceId)
        {
            this.DiagnosticServiceName = diagnosticServiceName;
            this.DiagnosticServiceId = diagnosticServiceId;
        }

        [DataMember]
        public string DiagnosticServiceName;

        [DataMember]
        public string DiagnosticServiceId;
    }
}
