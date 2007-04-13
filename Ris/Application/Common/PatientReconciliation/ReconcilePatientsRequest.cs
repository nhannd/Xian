using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Application.Common.PatientReconciliation
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
        [DataMember]
        public List<EntityRef> PatientRefs;
    }
}
