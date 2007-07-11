using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;

namespace ClearCanvas.Ris.Application.Common.PreviewService
{
    [DataContract]
    public class GetModalityProcedureStepResponse : DataContractBase
    {
        [DataMember]
        public PatientOrderData PatientOrderData;

        [DataMember]
        public List<DiagnosticServiceBreakdownSummary> DiagnosticServiceBreakdown;
    }
}
