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
        [Serializable]
        public enum ProbabilityScore
        {
            High,
            Moderate,
            Low
        }

        [DataMember]
        public PatientProfileSummary PatientProfile;

        [DataMember]
        public ProbabilityScore Score;
    }


}
