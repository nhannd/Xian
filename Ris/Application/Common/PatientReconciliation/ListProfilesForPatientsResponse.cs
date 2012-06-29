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
    public class ListProfilesForPatientsResponse : DataContractBase
    {
        public ListProfilesForPatientsResponse(List<PatientProfileSummary> profiles)
        {
            this.Profiles = profiles;
        }

        [DataMember]
        public List<PatientProfileSummary> Profiles;
    }
}
