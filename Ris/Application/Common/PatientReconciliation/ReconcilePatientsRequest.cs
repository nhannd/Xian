using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Application.Common.PatientReconcilliation
{
    [DataContract]
    public class ReconcilePatientsRequest : DataContractBase
    {
        public ReconcilePatientsRequest(List<EntityRef> patientRefs)
        {
            this.PatientRefs = patientRefs;
        }

        /// <summary>
        /// The set of patients to reconcile
        /// </summary>
        [DataMember(IsRequired = true)]
        public List<EntityRef> PatientRefs;
    }
}
