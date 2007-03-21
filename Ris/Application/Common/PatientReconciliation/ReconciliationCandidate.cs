using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.PatientReconciliation
{
    [DataContract]
    public class ReconciliationCandidate : DataContractBase
    {
        [DataContract]
        public enum ProbabilityScore
        {
            High,
            Moderate,
            Low
        }

        [DataMember(IsRequired=true)]
        public PatientProfileSummary PatientProfile;

        [DataMember(IsRequired = true)]
        public ProbabilityScore Score;
    }


}
