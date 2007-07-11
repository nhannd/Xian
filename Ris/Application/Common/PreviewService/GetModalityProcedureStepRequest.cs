using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.PreviewService
{
    [DataContract]
    public class GetModalityProcedureStepRequest : DataContractBase
    {
        [DataMember]
        public bool GetDiagnosticServiceBreakdown;
    }
}
