using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
    [DataContract]
    public class GetDiagnosticServiceSubTreeResponse : DataContractBase
    {
        public GetDiagnosticServiceSubTreeResponse(List<DiagnosticServiceTreeItem> diagnosticServiceSubTree)
        {
            this.DiagnosticServiceSubTree = diagnosticServiceSubTree;
        }

        [DataMember]
        public List<DiagnosticServiceTreeItem> DiagnosticServiceSubTree;
    }
}
