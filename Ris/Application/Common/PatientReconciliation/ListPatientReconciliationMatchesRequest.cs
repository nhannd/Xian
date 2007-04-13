using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.PatientReconciliation
{
    [DataContract]
    public class ListPatientReconciliationMatchesRequest : DataContractBase
    {
        public ListPatientReconciliationMatchesRequest(EntityRef patientProfileRef)
        {
            this.PatientProfileRef = patientProfileRef;
        }

        [DataMember]
        public EntityRef PatientProfileRef;
    }
}
