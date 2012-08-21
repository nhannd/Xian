#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common.Serialization;
using System.Runtime.Serialization;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Extended.Common.PatientReconciliation
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
