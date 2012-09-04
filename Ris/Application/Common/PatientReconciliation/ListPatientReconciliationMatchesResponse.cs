#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Application.Common.PatientReconciliation
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
