#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using System.Collections.Generic;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Extended.Common.PatientReconciliation
{
    [DataContract]
    public class ListPatientReconciliationMatchesResponse : DataContractBase
    {
        [DataMember]
        public List<PatientProfileSummary> ReconciledProfiles;

        [DataMember]
        public List<ReconciliationCandidate> MatchCandidates;
    }
}
