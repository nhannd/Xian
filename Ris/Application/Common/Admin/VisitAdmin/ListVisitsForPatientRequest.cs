using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.VisitAdmin
{
    [DataContract]
    public class ListVisitsForPatientRequest : DataContractBase
    {
        public ListVisitsForPatientRequest(EntityRef patientProfileRef)
        {
            this.PatientProfile = patientProfileRef;
        }

        [DataMember]
        public EntityRef PatientProfile;
    }
}
