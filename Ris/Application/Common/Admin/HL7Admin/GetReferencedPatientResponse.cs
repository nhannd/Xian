using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.HL7Admin
{
    [DataContract]
    public class GetReferencedPatientResponse : DataContractBase
    {
        public GetReferencedPatientResponse(EntityRef patientProfileRef)
        {
            PatientProfileRef = patientProfileRef;
        }

        public GetReferencedPatientResponse()
            : this(null)
        {
        }

        [DataMember]
        public EntityRef PatientProfileRef;
    }
}
