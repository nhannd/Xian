using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Application.Common.PatientReconcilliation
{
    [DataContract]
    public class ListPatientReconciliationMatchesResponse : DataContractBase
    {
        [DataMember]
        public List<PatientProfileSummary> ReconciledProfiles;

        [DataMember]
        public List<PatientProfileMatch> CandidateMatches;
    }
}
