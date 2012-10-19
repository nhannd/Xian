#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Application.Extended.Common.PatientReconciliation
{
    [DataContract]
    public class ListProfilesForPatientsRequest : DataContractBase
    {
        public ListProfilesForPatientsRequest(List<EntityRef> patientRefs)
        {
            this.PatientRefs = patientRefs;
        }

        /// <summary>
        /// The set of patients that will be reconciled
        /// </summary>
        [DataMember]
        public List<EntityRef> PatientRefs;
    }
}
